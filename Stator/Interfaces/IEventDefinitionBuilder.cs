using Stator.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Interfaces
{
    public interface IEventDefinitionBuilder<TEntity, TEntityState> where TEntity : class
    {
        IEventDefinitionBuilder<TEntity, TEntityState> WithTransitionMissHandler(Action<TEntity, IEvent<TEntity>> handler);

        ITransitionDefinitionBuilder<TEntity, TEntityState> SetTransition(TEntityState originalState, TEntityState destinationState);

        IStatorBuilder<TEntity, TEntityState> ConfirmEvent();
    }
}
