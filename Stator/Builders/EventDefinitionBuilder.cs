using Stator.BehaviorDefinitions;
using Stator.Interfaces;
using System;

namespace Stator.Builders
{
    public class EventDefinitionBuilder<TEntity, TEntityState> : IEventDefinitionBuilder<TEntity, TEntityState>
        where TEntity : class
    {
        private EventDefinition<TEntity, TEntityState> _eventDefinition;
        private StatorBuilder<TEntity, TEntityState> _statorBuilder;

        internal EventDefinitionBuilder(EventDefinition<TEntity, TEntityState> eventDefinition, StatorBuilder<TEntity, TEntityState> statorBuilder)
        {
            _eventDefinition = eventDefinition;
            _statorBuilder = statorBuilder;
        }
        /// <summary>
        /// Set delegate that will be invoked in case of allowed transitions mismatch
        /// </summary>
        /// <param name="handler">Delegate to invoke</param>
        public IEventDefinitionBuilder<TEntity, TEntityState> WithTransitionMissHandler(Action<TEntity, IEvent> handler)
        {
            _eventDefinition.RegisterTransitionMissHandler(handler);
            return this;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ITransitionDefinitionBuilder<TEntity, TEntityState> SetTransition(TEntityState originalState, TEntityState destinationState)
        {
            _eventDefinition.AddTransition(originalState, destinationState);
            return new TransitionDefinitionBuilder<TEntity, TEntityState>(_eventDefinition.GetTransitionDefinition(originalState), this);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IStatorBuilder<TEntity, TEntityState> ConfirmEvent()
            => _statorBuilder;
    }
}
