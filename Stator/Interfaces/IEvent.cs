using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Interfaces
{
    /// <summary>
    /// Event which can trigger a state transition
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Event args. It reachable from every handler or condition check
        /// </summary>
        object[] Args { get; }
    }
}
