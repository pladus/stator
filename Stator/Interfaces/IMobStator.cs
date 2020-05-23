using Stator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Interfaces
{
    public interface IMobStator
    {
        /// <summary>
        /// Attempts transition of Entity into new state
        /// </summary>
        /// <param name="event">Event which must triggered transition</param>
        /// <param name="mob">Entities group fir consistent transition</param>
        /// <returns></returns>
        MobTransitionResult Go(IEvent @event, params object[] mob);
    }

    public interface IMobStator<TEntity1, TEntity2>
        where TEntity1 : class
        where TEntity2 : class
    {
        /// <summary>
        /// Attempts transition of Entity into new state
        /// </summary>
        /// <param name="event">Event which must triggered transition</param>
        /// <param name="mob">Entities group fir consistent transition</param>
        /// <returns></returns>
        MobTransitionResult Go(IEvent @event, Tuple<TEntity1, TEntity2> mob);
    }

    public interface IMobStator<TEntity1, TEntity2, TEntity3>
    where TEntity1 : class
    where TEntity2 : class
    where TEntity3 : class
    {
        /// <summary>
        /// Attempts transition of Entity into new state
        /// </summary>
        /// <param name="event">Event which must triggered transition</param>
        /// <param name="mob">Entities group fir consistent transition</param>
        /// <returns></returns>
        MobTransitionResult Go(IEvent @event, Tuple<TEntity1, TEntity2, TEntity3> mob);
    }

    public interface IMobStator<TEntity1, TEntity2, TEntity3, TEntity4>
        where TEntity1 : class
        where TEntity2 : class
        where TEntity3 : class
        where TEntity4 : class
    {
        /// <summary>
        /// Attempts transition of Entity into new state
        /// </summary>
        /// <param name="event">Event which must triggered transition</param>
        /// <param name="mob">Entities group fir consistent transition</param>
        /// <returns></returns>
        MobTransitionResult Go(IEvent @event, Tuple<TEntity1, TEntity2, TEntity3, TEntity4> mob);
    }
}
