﻿using Stator.BehaviorDefinitions;
using Stator.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Builders
{
    public class TransitionDefinitionBuilder<TEntity, TEntityState> : ITransitionDefinitionBuilder<TEntity, TEntityState>, ITransitionDefinitionMatchBuilder<TEntity, TEntityState>
        where TEntity : class
    {
        private TransitionDefinition<TEntity, TEntityState> _transitionDefinition;
        private EventDefinitionBuilder<TEntity, TEntityState> _eventDefinitionBuilder;

        internal TransitionDefinitionBuilder(TransitionDefinition<TEntity, TEntityState> transitionDefinition, EventDefinitionBuilder<TEntity, TEntityState> eventDefinitionBuilder)
        {
            _transitionDefinition = transitionDefinition;
            _eventDefinitionBuilder = eventDefinitionBuilder;
        }

        /// <summary>
        /// Set a condition which must be succesfully checked before the transition will be allowed
        /// </summary>
        /// <param name="action">Predicate</param>
        public ITransitionDefinitionMatchBuilder<TEntity, TEntityState> Match(Func<TEntity, IEvent<TEntity>, bool>  action)
        {
            _transitionDefinition.RegisterTransitionConditionPredicate(action);
            return this;
        }
        /// <summary>
        /// Set a delegate which will be invoked in case of condition mismatch
        /// </summary>
        /// <param name="handler">Delegate to invoke</param>
        public ITransitionDefinitionBuilder<TEntity, TEntityState> Or(Action<TEntity, IEvent<TEntity>> handler)
        {
            _transitionDefinition.RegisterTransitionConditionFailedHandler(handler);
            return this;
        }

        public ITransitionDefinitionBuilder<TEntity, TEntityState> WithActionBeforeTransition(Action<TEntity, IEvent<TEntity>>  action)
        {
            _transitionDefinition.RegisterBeforeTransitionAction(action);
            return this;
        }

        public ITransitionDefinitionBuilder<TEntity, TEntityState> WithActionAfterTransition(Action<TEntity, IEvent<TEntity>>  action)
        {
            _transitionDefinition.RegisterAfterTransitionAction(action);
            return this;
        }

        public IEventDefinitionBuilder<TEntity, TEntityState> ConfirmTransition()
            => _eventDefinitionBuilder;

    }
}
