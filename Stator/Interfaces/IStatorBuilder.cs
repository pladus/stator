using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Stator.Interfaces
{
    public interface IStatorBuilder<TEntity, TEntityState> where TEntity : class
    {
        IEventDefinitionBuilder<TEntity, TEntityState> ForEvent<TEvent>() where TEvent : IEvent<TEntity>;
        Stator<TEntity, TEntityState> Build();
    }
}
