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

        private TransitionDefinition<TEntity, TEntityState> _initTransition;

        private Action<TEntity, IEvent<TEntity>> _transitionMissHandler;
        private Action<TEntity, TEntityState> _transitionAction;

        public EventDefinition(Action<TEntity, TEntityState> transitionAction, Action<TEntity,IEvent<TEntity>> transitionMissHandler = null)
        {
            _transitionMissHandler = transitionMissHandler;
            _transitionAction = transitionAction;
        }

        public void AddTransition(TEntityState originalState, TEntityState destinationState)
        {
            var definition = new TransitionDefinition<TEntity, TEntityState>
                (
                _transitionAction,
                originalState,
                destinationState
                );

            if (originalState == null)
                _initTransition = definition;
            else
                _transitionDefinitionMap[originalState] = definition;
        }

        internal TransitionDefinition<TEntity, TEntityState> GetTransitionDefinition(TEntityState originalState)
        {
            TransitionDefinition<TEntity, TEntityState> transitionDefinition;
            if (originalState == null && _initTransition != null)
            {
                transitionDefinition = _initTransition;
            }
            else if ((originalState == null && _initTransition == null)
                || !_transitionDefinitionMap.TryGetValue(originalState, out transitionDefinition))
                throw new ArgumentOutOfRangeException($"Definition for {originalState} not found. Please try to register it.");
            
            return transitionDefinition;
        }

        public TransitionResult<TEntity> PerformTransit(TEntity entity, TEntityState originalState, IEvent<TEntity> @event)
        {
            TransitionDefinition<TEntity, TEntityState> transitionDefinition;
            if (originalState == null && _initTransition != null)
            {
                transitionDefinition = _initTransition;
            }
            else if((originalState == null && _initTransition == null) 
                || !_transitionDefinitionMap.TryGetValue(originalState, out transitionDefinition))
            {
                _transitionMissHandler?.Invoke(entity, @event);
                return TransitionResult<TEntity>.MakeFailure(entity, FailureTypes.TransitionNotRegistered);
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
