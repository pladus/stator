using Stator.Interfaces;
using Stator.Models;
using System.Collections.Generic;

namespace Stator.Interfaces
{
    /// <summary>
    /// State machine with concrete settled event to using for multiple times
    /// </summary>
    public interface IStatorEventLift<TEntity> where TEntity : class
    {
        /// <summary>
        /// Event settled into the lift
        /// </summary>
        IEvent Event { get; }
        /// <summary>
        /// Rise event for state transition 
        /// </summary>
        /// <param name="entities">Entities for state transition</param>
        TransitionResult<TEntity>[] Go(IEnumerable<TEntity> entities);
        /// <summary>
        /// Rise event for state transition 
        /// </summary>
        /// <param name="entity">Entity for state transition</param>
        TransitionResult<TEntity> Go(TEntity entity);
    }
}