using Stator.Interfaces;
using Stator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stator
{
    public class StatorEventLift<TEntity, TEntityState> where TEntity : class
    {
        private readonly Stator<TEntity, TEntityState> _stator;
        private readonly IEvent<TEntity> _event;

        internal StatorEventLift(Stator<TEntity, TEntityState> stator, IEvent<TEntity> @event)
        {
            _stator = stator;
            _event = @event;
        }

        public IEvent<TEntity> Event { get => _event; }

        public TransitionResult<TEntity> Rise(TEntity entity)
        {
           return _stator.CommitTransition(entity, _event);
        }

        public TransitionResult<TEntity>[] Rise(IEnumerable<TEntity> entities)
        {
            return entities.Select(x => _stator.CommitTransition(x, _event)).ToArray();
        }
    }
}
