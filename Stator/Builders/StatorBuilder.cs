using Stator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Stator.Builders
{
    public class StatorBuilder<TEntity, TEntityState> : IStatorBuilder<TEntity, TEntityState>, INotInittedStatorBuilder<TEntity, TEntityState>
        where TEntity : class
    {
        private readonly Stator<TEntity, TEntityState> _stator;

        internal StatorBuilder(Stator<TEntity, TEntityState> stator)
        {
            _stator = stator;
        }

        /// <summary>
        /// Select a State property for controlling by Stator
        /// </summary>
        /// <param name="statusPropertySelector">Property selector</param>
        public IStatorBuilder<TEntity, TEntityState> State(Expression<Func<TEntity, TEntityState>> statusPropertySelector)
        {
            _stator.SelectStatusProperty(statusPropertySelector);
            return this;
        }

        /// <summary>
        /// Register Event which can triggered state transitions
        /// </summary>
        /// <typeparam name="TEvent">Event type. Must implement Stator.IEvent<T>.</typeparam>
        public IEventDefinitionBuilder<TEntity, TEntityState> ForEvent<TEvent>() where TEvent : IEvent<TEntity>
        => new EventDefinitionBuilder<TEntity, TEntityState>(_stator.GetEventDefinition<TEvent>(), this);
        
        /// <summary>
        /// Finish stator configuring
        /// </summary>
        public Stator<TEntity, TEntityState> Build()
        {
            return _stator;
        }
    }
}
