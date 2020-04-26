using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Interfaces
{
    public interface ITransitionDefinitionMatchBuilder<TEntity, TEntityState> where TEntity : class
    {
        ITransitionDefinitionBuilder<TEntity, TEntityState> Or(Action<TEntity, IEvent<TEntity>> handler);
    }
}
