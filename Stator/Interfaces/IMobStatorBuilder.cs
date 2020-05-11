using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Interfaces
{
    public interface IMobStatorBuilder
    {
        IMobStatorBuilder<TEntity> AddStateMachine<TEntity, TEntityState>(Stator<TEntity, TEntityState> stator) where TEntity : class;
    }
    public interface IMobStatorBuilder<TEntity1> where TEntity1 : class
    {
        IMobStatorBuilder<TEntity1, TEntity2> AddStateMachine<TEntity2, TEntityState>(Stator<TEntity2, TEntityState> stator) where TEntity2 : class;
    }

    public interface IMobStatorBuilder<TEntity1, TEntity2>
        where TEntity1 : class
        where TEntity2 : class
    {
        IMobStatorBuilder<TEntity1, TEntity2, TEntity3> AddStateMachine<TEntity3, TEntityState>(Stator<TEntity3, TEntityState> stator) where TEntity3 : class;
        IMobStator<TEntity1, TEntity2> Build();
    }

    public interface IMobStatorBuilder<TEntity1, TEntity2, TEntity3>
        where TEntity1 : class
        where TEntity2 : class
        where TEntity3 : class
    {
        IMobStatorBuilder<TEntity1, TEntity2, TEntity3, TEntity4> AddStateMachine<TEntity4, TEntityState>(Stator<TEntity4, TEntityState> stator) where TEntity4 : class;
        IMobStator<TEntity1, TEntity2, TEntity3> Build();
    }

    public interface IMobStatorBuilder<TEntity1, TEntity2, TEntity3, TEntity4>
        where TEntity1 : class
        where TEntity2 : class
        where TEntity3 : class
        where TEntity4 : class
    {
        IMobStatorExpandedBuilder AddStateMachine<TEntity, TEntityState>(Stator<TEntity, TEntityState> stator) where TEntity : class;
        IMobStator<TEntity1, TEntity2, TEntity3, TEntity4> Build();
    }
    public interface IMobStatorExpandedBuilder
    {
        IMobStator Build();
        IMobStatorExpandedBuilder AddStateMachine<TEntity, TEntityState>(Stator<TEntity, TEntityState> stator) where TEntity : class;
    }
}
