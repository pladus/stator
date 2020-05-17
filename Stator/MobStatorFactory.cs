using Stator.Builders;
using Stator.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stator
{
    public static class MobStatorFactory
    {
        public static IMobStatorBuilder<TEntity> InitWithStator<TEntity, TEntityState>(Stator<TEntity, TEntityState> stator, bool withRollbackOnFailure = false)
            where TEntity : class
        {
            var mobStatorBase = new MobStatorBase(withRollbackOnFailure);
            mobStatorBase.AddStatorNode(stator);
            return new MobStatorBuilder<TEntity, TEntityState>(mobStatorBase);
        }
    }
}
