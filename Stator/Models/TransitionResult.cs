using Stator.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Models
{
    public class TransitionResult<TEntity> where TEntity : class
    {
        internal TransitionResult(TEntity entity)
        {
            Entity = entity;
            FailureType = FailureTypes.None;
            Success = true;
        }
        private TransitionResult() { }

        internal static TransitionResult<TEntity> MakeFailure(TEntity entity, FailureTypes failureType)
        => new TransitionResult<TEntity> { Success = false, Entity = entity, FailureType = failureType };

        /// <summary>
        /// Entity for state transition
        /// </summary>
        public TEntity Entity { get; private set; }
        /// <summary>
        /// Describes what really is going wrong
        /// </summary>
        public FailureTypes FailureType { get; private set; }
        /// <summary>
        /// Shows is transition was successful
        /// </summary>
        public bool Success { get; private set; }
    }
}
