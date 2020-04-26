using Stator.Enums;
using Stator.Interfaces;
using Stator.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Stator.BehaviorDefinitions
{
    internal class EventDefinition<TEntity, TEntityState> where TEntity : class
    {

        private ConcurrentDictionary<TEntityState, TransitionDefinition<TEntity, TEntityState>> _transitionDefinitionMap 
            = new ConcurrentDictionary<TEntityState, TransitionDefinition<TEntity, TEntityState>>();

        private Action<TEntity, IEvent<TEntity>> _transitionMissHandler;
        private Action<TEntity, TEntityState> _transitionAction;

        public EventDefinition(Action<TEntity, TEntityState> transitionAction, Action<TEntity,IEvent<TEntity>> transitionMissHandler = null)
        {
            _transitionMissHandler = transitionMissHandler;
            _transitionAction = transitionAction;
        }

        public void AddTransition(TEntityState originalState, TEntityState destinationState)
        {
            _transitionDefinitionMap[originalState] = new TransitionDefinition<TEntity, TEntityState>
                (
                _transitionAction,
                originalState,
                destinationState
                );
        }

        internal TransitionDefinition<TEntity, TEntityState> GetTransitionDefinition(TEntityState originalState)
        {
            if (!_transitionDefinitionMap.TryGetValue(originalState, out var definition))
                throw new ArgumentOutOfRangeException($"Definition for {originalState} not found. Please try to register it.");
            
            return definition;
        }

        public TransitionResult<TEntity> PerformTransit(TEntity entity, TEntityState originalState, IEvent<TEntity> @event)
        {
            if(!_transitionDefinitionMap.TryGetValue(originalState, out var transitionDefinition))
            {
                _transitionMissHandler?.Invoke(entity, @event);
                return new TransitionResult<TEntity>(entity, false, FailureTypes.TransitionNotRegistered);
            }

            transitionDefinition.PerformTransition(entity, @event);
            return new TransitionResult<TEntity>(entity);
        }

        public void RegisterTransitionMissHandler(Action<TEntity, IEvent<TEntity>> handler)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler), "Handler can't be null.");

            _transitionMissHandler = handler;
        }

        public void TransitionMissHandle(TEntity entity, IEvent<TEntity> @event)
        {
            _transitionMissHandler?.Invoke(entity, @event);
        }

        public void RegisterTransitionConditionPredicate(TEntityState originalState, Func<TEntity, IEvent<TEntity>, bool> predicate)
        {
            var transitionDefinition = GetTransitionDefinition(originalState);
            transitionDefinition.RegisterTransitionConditionPredicate(predicate);
        }

        public void RegisterBeforeTransitionAction(TEntityState originalState, Action<TEntity, IEvent<TEntity>> action)
        {
            var transitionDefinition = GetTransitionDefinition(originalState);
            transitionDefinition.RegisterBeforeTransitionAction(action);
        }

        public void RegisterAfterTransitionAction(TEntityState originalState, Action<TEntity, IEvent<TEntity>> action)
        {
            var transitionDefinition = GetTransitionDefinition(originalState);
            transitionDefinition.RegisterAfterTransitionAction(action);
        }

        public void RegisterTransitionConditionFailedHandler(TEntityState originalState, Action<TEntity, IEvent<TEntity>> handler)
        {
            var transitionDefinition = GetTransitionDefinition(originalState);
            transitionDefinition.RegisterTransitionConditionFailedHandler(handler);
        }
    }
}
