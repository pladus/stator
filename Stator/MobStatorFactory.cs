using Stator.Builders;
using Stator.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stator
{
    public static class MobStatorFactory
    {
        public static IMobStatorBuilder<TEntity> InitWithStator<TEntity, TEntityState>(Stator<TEntity, TEntityState> stator)
            where TEntity : class
        {
            var mobStatorBase = new MobStatorBase();
            mobStatorBase.AddStatorNode(stator);
            return new MobStatorBuilder<TEntity, TEntityState>(mobStatorBase);
        }
    }
}
