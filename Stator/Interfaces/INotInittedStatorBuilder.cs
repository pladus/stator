using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Stator.Interfaces
{
    public interface INotInittedStatorBuilder<TEntity, TEntityState> where TEntity : class
    {
        IStatorBuilder<TEntity, TEntityState> State(Expression<Func<TEntity, TEntityState>> statusPropertySelector);
    }
}
