using Stator.BehaviorDefinitions;
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
    public class Stator<TEntity, TEntityState> where TEntity : class
    {
        private PropertyInfo _statusPropertyInfo;
        private ConcurrentDictionary<Type, EventDefinition<TEntity, TEntityState>>
            _eventDefinitionMap = new ConcurrentDictionary<Type, EventDefinition<TEntity, TEntityState>>();
        private Func<TEntity, TEntityState> _getStateFunc;
        private Action<TEntity, TEntityState> _setStateAction;
        private Action<IEvent<TEntity>, TEntity> _eventNotRegisteredHandler;

        public static INotInittedStatorBuilder<TEntity, TEntityState> InitStator()
        {
            return new StatorBuilder<TEntity, TEntityState>(new Stator<TEntity, TEntityState>());
        }
        internal void SelectStatusProperty(Expression<Func<TEntity, TEntityState>> statusPropertySelector)
        {
            InitStatorWithSelector(statusPropertySelector);
        }

        public Stator(Expression<Func<TEntity, TEntityState>> statusPropertySelector)
        {
            InitStatorWithSelector(statusPropertySelector);
        }

        internal EventDefinition<TEntity, TEntityState> GetEventDefinition<TEvent>() where TEvent : IEvent<TEntity>
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
            var methodInfo = property.GetType().GetMethod("SetValue", new Type[] { typeof(System.Object), typeof(object) });
            var paramEntityExpression = Expression.Parameter(typeof(TEntity), "entity");
            var paramNewStateExpression = Expression.Parameter(typeof(TEntityState), "newState");
            var boxedNewStateExpression = Expression.Convert(paramNewStateExpression, typeof(object));
            var constantObject = Expression.Constant(property);
            var setExpression = Expression.Call(constantObject, methodInfo, new Expression[] { paramEntityExpression, boxedNewStateExpression });

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

        public void RegisterTransition<TEvent>(TEntityState originalState, TEntityState destinationState) where TEvent : IEvent<TEntity>
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

        public void RegisterTransitionMissHandler<TEvent>(Action<TEntity, IEvent<TEntity>> handler) where TEvent : IEvent<TEntity>
        {
            var eventType = typeof(TEvent);
            var eventDefinition = GetEventDefinition(eventType);
            eventDefinition.RegisterTransitionMissHandler(handler);
        }

        public void RegisterAfterTransitionAction<TEvent>(TEntityState originalState, Action<TEntity, IEvent<TEntity>> action) where TEvent : IEvent<TEntity>
        {
            var eventType = typeof(TEvent);
            var eventDefinition = GetEventDefinition(eventType);
            eventDefinition.RegisterAfterTransitionAction(originalState, action);
        }

        public void RegisterBeforeTransitionAction<TEvent>(TEntityState originalState, Action<TEntity, IEvent<TEntity>> action) where TEvent : IEvent<TEntity>
        {
            var eventType = typeof(TEvent);
            var eventDefinition = GetEventDefinition(eventType);
            eventDefinition.RegisterBeforeTransitionAction(originalState, action);
        }

        public void RegisterTransitionConditionPredicate<TEvent>(TEntityState originalState, Func<TEntity, IEvent<TEntity>, bool> predicate) where TEvent : IEvent<TEntity>
        {
            var eventType = typeof(TEvent);
            var eventDefinition = GetEventDefinition(eventType);
            eventDefinition.RegisterTransitionConditionPredicate(originalState, predicate);
        }

        public void RegisterTransitionConditionFailedHandler<TEvent>(TEntityState originalState, Action<TEntity, IEvent<TEntity>> handler) where TEvent : IEvent<TEntity>
        {
            var eventType = typeof(TEvent);
            var eventDefinition = GetEventDefinition(eventType);
            eventDefinition.RegisterTransitionConditionFailedHandler(originalState, handler);
        }

        private EventDefinition<TEntity, TEntityState> GetEventDefinition(Type eventType)
        {
            if (!_eventDefinitionMap.ContainsKey(eventType))
                _eventDefinitionMap.TryAdd(eventType, new EventDefinition<TEntity, TEntityState>
                    (
                    _setStateAction
                    ));

            var eventDefinition = _eventDefinitionMap[eventType];
            return eventDefinition;
        }

        public TransitionResult<TEntity> CommitTransition(TEntity entity, IEvent<TEntity> @event)
        {
            var eventType = @event.GetType();
            if (!_eventDefinitionMap.ContainsKey(eventType))
            {
                _eventNotRegisteredHandler?.Invoke(@event, entity);
                return new TransitionResult<TEntity>(@event.Item, false, FailureTypes.EventNotRegistered);
            }

            var eventDefinition = _eventDefinitionMap[eventType];

            var currentState = _getStateFunc(entity);

            return eventDefinition.PerformTransit(entity, currentState, @event);
        }

        public StatorEventLift<TEntity, TEntityState> GetEventLift(IEvent<TEntity> @event)
         => new StatorEventLift<TEntity, TEntityState>(this, @event);
    }
}
