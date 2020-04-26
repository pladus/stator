using Stator.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Models
{
    public class TransitionResult<TEntity> where TEntity: class
    {
        public TransitionResult(TEntity entity, bool success = true, FailureTypes failureType = FailureTypes.None)
        {
            Entity = entity;
            FailureType = failureType;
            Success = success;
        }

        public TEntity Entity { get; private set; }
        public FailureTypes FailureType { get; private set; }
        public bool Success { get; private set; }
    }
}
