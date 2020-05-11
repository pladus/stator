using Stator.Interfaces;
using System;
using System.Collections.Concurrent;

namespace Stator
{
    public abstract class MobStatorBase
    {
        protected ConcurrentDictionary<Type, Func<object, object, dynamic>> _mobStators
            = new ConcurrentDictionary<Type, Func<object, object, dynamic>>();

        internal void AddStatorNode<TEntity, TEntityState>(IStator<TEntity, TEntityState> stator) where TEntity : class
        {
            var nodeType = typeof(TEntity);
            _mobStators[nodeType] = (entity, @event) => stator.Go((TEntity)entity, (IEvent)@event);
        }
    }
}