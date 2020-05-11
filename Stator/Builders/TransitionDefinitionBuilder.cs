using Stator.BehaviorDefinitions;
using Stator.Interfaces;
using System;

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
        /// <inheritdoc/>
        /// </summary>
        public ITransitionDefinitionMatchBuilder<TEntity, TEntityState> Match(Func<TEntity, IEvent, bool> action)
        {
            _transitionDefinition.RegisterTransitionConditionPredicate(action);
            return this;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ITransitionDefinitionBuilder<TEntity, TEntityState> Or(Action<TEntity, IEvent> handler)
        {
            _transitionDefinition.RegisterTransitionConditionFailedHandler(handler);
            return this;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ITransitionDefinitionBuilder<TEntity, TEntityState> WithActionBeforeTransition(Action<TEntity, IEvent> action)
        {
            _transitionDefinition.RegisterBeforeTransitionAction(action);
            return this;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ITransitionDefinitionBuilder<TEntity, TEntityState> WithActionAfterTransition(Action<TEntity, IEvent> action)
        {
            _transitionDefinition.RegisterAfterTransitionAction(action);
            return this;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEventDefinitionBuilder<TEntity, TEntityState> ConfirmTransition()
            => _eventDefinitionBuilder;

    }
}
