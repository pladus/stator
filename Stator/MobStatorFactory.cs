using Stator.Builders;
using Stator.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stator
{
    public static class MobStatorFactory
    {
        /// <summary>
        /// Starts grouping linked state machines for consistent state transitions
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TEntityState"></typeparam>
        /// <param name="stator">First state machine</param>
        /// <param name="withRollbackOnFailure">If true, mob state machine will attempt to rollback new states for all processed wntities since failed transition </param>
        /// <returns></returns>
        public static IMobStatorBuilder<TEntity> InitWithStator<TEntity, TEntityState>(Stator<TEntity, TEntityState> stator, bool withRollbackOnFailure = false)
            where TEntity : class
        {
            var mobStatorBase = new MobStatorBase(withRollbackOnFailure);
            mobStatorBase.AddStatorNode(stator);
            return new MobStatorBuilder<TEntity, TEntityState>(mobStatorBase);
        }
    }
}
