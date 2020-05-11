using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Stator.Interfaces
{
    public interface IStatorBuilder<TEntity, TEntityState> where TEntity : class
    {
        /// <summary>
        /// Register Event which can triggered state transitions
        /// </summary>
        /// <typeparam name="TEvent">Event type. Must implement Stator.IEvent.</typeparam>
        IEventDefinitionBuilder<TEntity, TEntityState> ForEvent<TEvent>() where TEvent : IEvent<TEntity>;
        /// <summary>
        /// Finish stator configuring and get completed state machine
        /// </summary>
        Stator<TEntity, TEntityState> Build();
    }
}
