using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Interfaces
{
    public interface ITransitionDefinitionMatchBuilder<TEntity, TEntityState> where TEntity : class
    {
        /// <summary>
        /// Set a delegate which will be invoked in case of condition mismatch
        /// </summary>
        /// <param name="handler">Delegate to invoke</param>
        ITransitionDefinitionBuilder<TEntity, TEntityState> Or(Action<TEntity, IEvent> handler);
    }
}
