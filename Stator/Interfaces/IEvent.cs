using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Interfaces
{
    /// <summary>
    /// Event which can trigger a state transition
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IEvent<TEntity> where TEntity: class
    {
        /// <summary>
        /// Entity. It reachable from every handler or condition check
        /// </summary>
        TEntity Item { get; }
        /// <summary>
        /// Event args. It reachable from every handler or condition check
        /// </summary>
        object[] Args { get; }
    }
}
