using Stator.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Interfaces
{
    public interface IEventDefinitionBuilder<TEntity, TEntityState> where TEntity : class
    {
        /// <summary>
        /// Set delegate that will be invoked in case of allowed transitions mismatch
        /// </summary>
        /// <param name="handler">Delegate to invoke</param>
        IEventDefinitionBuilder<TEntity, TEntityState> WithTransitionMissHandler(Action<TEntity, IEvent> handler);
        /// <summary>
        /// Register allowed state transition associated with Event
        /// </summary>
        /// <param name="originalState">From this state</param>
        /// <param name="destinationState">To this state</param>
        ITransitionDefinitionBuilder<TEntity, TEntityState> SetTransition(TEntityState originalState, TEntityState destinationState);
        /// <summary>
        /// Finish event configuring and return to new event registering
        /// </summary>
        IStatorBuilder<TEntity, TEntityState> ConfirmEvent();
    }
}
