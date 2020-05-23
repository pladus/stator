using Stator.Interfaces;
using Stator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stator
{
    public class StatorEventLift<TEntity, TEntityState> : IStatorEventLift<TEntity> where TEntity : class
    {
        private readonly Stator<TEntity, TEntityState> _stator;
        private readonly IEvent _event;

        internal StatorEventLift(Stator<TEntity, TEntityState> stator, IEvent @event)
        {
            _stator = stator;
            _event = @event;
        }

        public IEvent Event { get => _event; }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public TransitionResult<TEntity> Go(TEntity entity)
        {
            return _stator.Go(entity, _event);
        }
        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public TransitionResult<TEntity>[] Go(IEnumerable<TEntity> entities)
        {
            return entities.Select(x => _stator.Go(x, _event)).ToArray();
        }
    }
}
