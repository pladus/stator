using Stator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Stator.Builders
{
    public class StatorBuilder<TEntity, TEntityState> : IStatorBuilder<TEntity, TEntityState>, INotInittedStatorBuilder<TEntity, TEntityState>
        where TEntity : class
    {
        private readonly Stator<TEntity, TEntityState> _stator;

        public StatorBuilder(Stator<TEntity, TEntityState> stator)
        {
            _stator = stator;
        }

        public IStatorBuilder<TEntity, TEntityState> Status(Expression<Func<TEntity, TEntityState>> statusPropertySelector)
        {
            _stator.SelectStatusProperty(statusPropertySelector);
            return this;
        }

        public IEventDefinitionBuilder<TEntity, TEntityState> ForEvent<TEvent>() where TEvent : IEvent<TEntity>
        => new EventDefinitionBuilder<TEntity, TEntityState>(_stator.GetEventDefinition<TEvent>(), this);
        

        public Stator<TEntity, TEntityState> Build()
        {
            return _stator;
        }
    }
}
