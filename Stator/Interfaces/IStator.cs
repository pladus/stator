using Stator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Interfaces
{
    public interface IStator<TEntity, TEntityState>
        where TEntity : class

    {
        /// <summary>
        /// Attempts transition of Entity into new state
        /// </summary>
        /// <param name="entity">Entity to transition</param>
        /// <param name="event">Event which must triggered transition</param>
        /// <param name="restoreOnFailure">Forces the state machine to try restore consistency on transition failure.</param>
        TransitionResult<TEntity> Go(TEntity entity, IEvent @event, bool restoreOnFailure = false);
        /// <summary>
        /// Returns state machine with concrete settled event to using for multiple times
        /// </summary>
        /// <param name="event">Event to settle into the lift</param>
        StatorEventLift<TEntity, TEntityState> GetEventLift(IEvent @event);
        internal Func<TEntity, TEntityState> GetGetter();
        internal Action<TEntity, TEntityState> GetSetter();
    }
}
