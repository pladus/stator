using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Interfaces
{
    public interface ITransitionDefinitionBuilder<TEntity, TEntityState> where TEntity : class
    {
        ITransitionDefinitionMatchBuilder<TEntity, TEntityState> Match(Func<TEntity, IEvent<TEntity>, bool> action);
        ITransitionDefinitionBuilder<TEntity, TEntityState> WithActionBeforeTransition(Action<TEntity, IEvent<TEntity>> action);
        ITransitionDefinitionBuilder<TEntity, TEntityState> WithActionAfterTransition(Action<TEntity, IEvent<TEntity>> action);
        IEventDefinitionBuilder<TEntity, TEntityState> ConfirmTransition();
    }
}
