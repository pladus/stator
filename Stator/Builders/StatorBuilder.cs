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
        /// <inheritdoc/>
        /// </summary>
        public IStatorBuilder<TEntity, TEntityState> State(Expression<Func<TEntity, TEntityState>> statusPropertySelector)
        {
            _stator.SelectStatusProperty(statusPropertySelector);
            return this;
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public IEventDefinitionBuilder<TEntity, TEntityState> ForEvent<TEvent>() where TEvent : IEvent
        => new EventDefinitionBuilder<TEntity, TEntityState>(_stator.GetEventDefinition<TEvent>(), this);
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public Stator<TEntity, TEntityState> Build()
        {
            return _stator;
        }
    }
}
