using Stator.Interfaces;
using System;
using System.Collections.Concurrent;

namespace Stator
{
    public class MobStatorBase
    {
        internal ConcurrentDictionary<Type, Func<object, object, object>> Stators;
        public MobStatorBase() => Stators = new ConcurrentDictionary<Type, Func<object, object, object>>();
        public MobStatorBase(ConcurrentDictionary<Type, Func<object, object, object>> stators) => Stators = stators;

        internal void AddStatorNode<TEntity, TEntityState>(IStator<TEntity, TEntityState> stator) where TEntity : class
        {
            var nodeType = typeof(TEntity);
            Stators[nodeType] = (entity, @event) => stator.Go((TEntity)entity, (IEvent)@event);
        }
    }
}