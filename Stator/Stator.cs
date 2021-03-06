﻿using Stator.BehaviorDefinitions;
using Stator.Builders;
using Stator.Enums;
using Stator.Interfaces;
using Stator.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Stator
{
    public class Stator<TEntity, TEntityState> : IStator<TEntity, TEntityState>
        where TEntity : class
    {
        private PropertyInfo _statusPropertyInfo;
        private ConcurrentDictionary<Type, EventDefinition<TEntity, TEntityState>>
            _eventDefinitionMap = new ConcurrentDictionary<Type, EventDefinition<TEntity, TEntityState>>();
        private Func<TEntity, TEntityState> _getStateFunc;
        private Action<TEntity, TEntityState> _setStateAction;
        private Action<IEvent, TEntity> _eventNotRegisteredHandler;

        /// <summary>
        /// Starts state machine configuring
        /// </summary>
        /// <returns>Fluent builder</returns>
        public static INotInittedStatorBuilder<TEntity, TEntityState> InitStator()
        {
            return new StatorBuilder<TEntity, TEntityState>(new Stator<TEntity, TEntityState>());
        }
        internal void SelectStatusProperty(Expression<Func<TEntity, TEntityState>> statusPropertySelector)
        {
            InitStatorWithSelector(statusPropertySelector);
        }

        internal Stator(Expression<Func<TEntity, TEntityState>> statusPropertySelector)
        {
            InitStatorWithSelector(statusPropertySelector);
        }

        internal EventDefinition<TEntity, TEntityState> GetEventDefinition<TEvent>() where TEvent : IEvent
            => GetEventDefinition(typeof(TEvent));

        private Stator()
        {
        }

        private void InitStatorWithSelector(Expression<Func<TEntity, TEntityState>> statusPropertySelector)
        {

            var lambda = statusPropertySelector as LambdaExpression;

            if (lambda == null)
                throw new ArgumentException("Selector of the status property must be lambda expression. Example: x => x.Status", nameof(statusPropertySelector));

            var propertyAccess = GetPropertyAccessExpression(lambda.Body);

            if (propertyAccess == null)
                throw new ArgumentException("Selector of the status property must be property access expression. Example: x => x.Status", nameof(statusPropertySelector));


            if (propertyAccess.Member.DeclaringType == null)
                throw new ArgumentException("Selected property must have declared type.");


            _getStateFunc = statusPropertySelector.Compile();
            _statusPropertyInfo = propertyAccess.Member.DeclaringType.GetProperty(propertyAccess.Member.Name);
            if (_statusPropertyInfo == null)
                throw new ArgumentException("Selected member must be a class property.");

            _setStateAction = GetFuncForSetStatus(_statusPropertyInfo);
        }

        private Action<TEntity, TEntityState> GetFuncForSetStatus(PropertyInfo property)
        {
            var methodInfo = property.GetSetMethod();

            var paramEntityExpression = Expression.Parameter(typeof(TEntity), "entity");
            var paramNewStateExpression = Expression.Parameter(typeof(TEntityState), "newState");
            var setExpression = Expression.Call(paramEntityExpression, methodInfo, new Expression[] { paramNewStateExpression });

            var resultLambda = Expression.Lambda(setExpression, new ParameterExpression[] { paramEntityExpression, paramNewStateExpression });

            return ((Expression<Action<TEntity, TEntityState>>)resultLambda).Compile();
        }

        private static MemberExpression GetPropertyAccessExpression(Expression expression)
        {
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                return ((MemberExpression)expression);
            }

            if (expression.NodeType == ExpressionType.Convert)
            {
                var operand = ((UnaryExpression)expression).Operand;
                return GetPropertyAccessExpression(operand);
            }

            return null;
        }

        internal void RegisterTransition<TEvent>(TEntityState originalState, TEntityState destinationState) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);

            if (!_eventDefinitionMap.ContainsKey(eventType))
                _eventDefinitionMap.TryAdd(eventType, new EventDefinition<TEntity, TEntityState>
                    (
                    _setStateAction
                    ));

            var eventDefinition = _eventDefinitionMap[eventType];

            eventDefinition.AddTransition(originalState, destinationState);
        }

        internal void RegisterTransitionMissHandler<TEvent>(Action<TEntity, IEvent> handler) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            var eventDefinition = GetEventDefinition(eventType);
            eventDefinition.RegisterTransitionMissHandler(handler);
        }

        internal void RegisterAfterTransitionAction<TEvent>(TEntityState originalState, Action<TEntity, IEvent> action) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            var eventDefinition = GetEventDefinition(eventType);
            eventDefinition.RegisterAfterTransitionAction(originalState, action);
        }

        internal void RegisterBeforeTransitionAction<TEvent>(TEntityState originalState, Action<TEntity, IEvent> action) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            var eventDefinition = GetEventDefinition(eventType);
            eventDefinition.RegisterBeforeTransitionAction(originalState, action);
        }

        internal void RegisterTransitionConditionPredicate<TEvent>(TEntityState originalState, Func<TEntity, IEvent, bool> predicate) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            var eventDefinition = GetEventDefinition(eventType);
            eventDefinition.RegisterTransitionConditionPredicate(originalState, predicate);
        }

        internal void RegisterTransitionConditionFailedHandler<TEvent>(TEntityState originalState, Action<TEntity, IEvent> handler) where TEvent : IEvent
        {
            var eventType = typeof(TEvent);
            var eventDefinition = GetEventDefinition(eventType);
            eventDefinition.RegisterTransitionConditionFailedHandler(originalState, handler);
        }

        internal EventDefinition<TEntity, TEntityState> GetEventDefinition(Type eventType)
        {
            if (!_eventDefinitionMap.ContainsKey(eventType))
                _eventDefinitionMap.TryAdd(eventType, new EventDefinition<TEntity, TEntityState>
                    (
                    _setStateAction
                    ));

            var eventDefinition = _eventDefinitionMap[eventType];
            return eventDefinition;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public TransitionResult<TEntity> Go(TEntity entity, IEvent @event, bool restoreOnFailure = false)
        {
            if (!restoreOnFailure)
                return PerformTransition(entity, @event);

            var previousState = _getStateFunc(entity);
            try
            {
                return PerformTransition(entity, @event);
            }
            catch
            {
                _setStateAction(entity, previousState);
                throw;
            }
        }
        Func<TEntity, TEntityState> IStator<TEntity, TEntityState>.GetGetter()
            => _getStateFunc;
        Action<TEntity, TEntityState> IStator<TEntity, TEntityState>.GetSetter()
            => _setStateAction;       

        private TransitionResult<TEntity> PerformTransition(TEntity entity, IEvent @event)
        {
            var eventType = @event.GetType();
            if (!_eventDefinitionMap.ContainsKey(eventType))
            {
                _eventNotRegisteredHandler?.Invoke(@event, entity);
                return TransitionResult<TEntity>.MakeFailure(entity, FailureTypes.EventNotRegistered);
            }

            var eventDefinition = _eventDefinitionMap[eventType];

            var currentState = _getStateFunc(entity);

            return eventDefinition.PerformTransit(entity, currentState, @event);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public StatorEventLift<TEntity, TEntityState> GetEventLift(IEvent @event)
         => new StatorEventLift<TEntity, TEntityState>(this, @event);
    }
}
