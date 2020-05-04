using Stator.Enums;
using Stator.Interfaces;
using Stator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.BehaviorDefinitions
{
    internal class TransitionDefinition<TEntity, TEntityState> where TEntity : class
    {
        private Action<TEntity, IEvent<TEntity>> _beforeTransitionAction;
        private Action<TEntity, IEvent<TEntity>> _afterTransitionAction;
        private Func<TEntity, IEvent<TEntity>, bool> _transitionConditionPredicate;
        private Action<TEntity, IEvent<TEntity>> _transitionConditionFailedHandler;
        private Action<TEntity, TEntityState> _transitionAction;

        public TEntityState OriginalState { get; private set; }
        private readonly TEntityState _destinationState;

        public TransitionDefinition(
            Action<TEntity, TEntityState> transitionAction,
            TEntityState originalState,
            TEntityState destinationState,
            Func<TEntity, IEvent<TEntity>, bool> transitionCondition = null,
            Action<TEntity, IEvent<TEntity>> transitionConditionFailedHandler = null,
            Action<TEntity, IEvent<TEntity>> beforeTransitionAction = null,
            Action<TEntity, IEvent<TEntity>> afterTransitionAction = null
            )
        {
            _beforeTransitionAction = beforeTransitionAction;
            _afterTransitionAction = afterTransitionAction;
            _transitionConditionPredicate = transitionCondition;
            _transitionConditionFailedHandler = transitionConditionFailedHandler;
            _transitionAction = transitionAction;
            OriginalState = originalState;
            _destinationState = destinationState;
        }

        public TransitionResult<TEntity> PerformTransition(TEntity entity, IEvent<TEntity> @event)
        {
            if (!CanPerformTransition(entity, @event))
            {
                _transitionConditionFailedHandler?.Invoke(entity, @event);
                return new TransitionResult<TEntity> (entity, false, FailureTypes.TransitionConditionFailed);
            }

            _beforeTransitionAction?.Invoke(entity, @event);
            _transitionAction?.Invoke(entity, _destinationState);
            _afterTransitionAction?.Invoke(entity, @event);

            return new TransitionResult<TEntity>(entity);
        }

        private bool CanPerformTransition(TEntity entity, IEvent<TEntity> @event)
        {
            if (_transitionConditionPredicate == null)
                return true;

            return _transitionConditionPredicate(entity, @event);
        }

        public void RegisterTransitionConditionPredicate(Func<TEntity, IEvent<TEntity>, bool> predicate)
        {
            _transitionConditionPredicate = predicate;
        }

        public void RegisterBeforeTransitionAction(Action<TEntity, IEvent<TEntity>> action)
        {
            _beforeTransitionAction = action;
        }

        public void RegisterAfterTransitionAction(Action<TEntity, IEvent<TEntity>> action)
        {
            _afterTransitionAction = action;
        }

        public void RegisterTransitionConditionFailedHandler(Action<TEntity, IEvent<TEntity>> handler)
        {
            _transitionConditionFailedHandler = handler;
        }
    }
}
