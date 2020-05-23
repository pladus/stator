using Stator.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Models
{
    public class MobTransitionResult
    {
        internal MobTransitionResult(Type[] successedEntityTypes) => SuccessedEntityTypes = successedEntityTypes;

        private MobTransitionResult() { }
        /// <summary>
        /// Transition result
        /// </summary>
        public bool Success { get; private set; } = true;
        /// <summary>
        /// Failure types per entity type
        /// </summary>
        public IDictionary<Type, FailureTypes> TypeToFailureTypesMap { get; private set; }

        /// <summary>
        /// Types of Entities which were moved into new state
        /// </summary>
        public Type[] SuccessedEntityTypes { get; private set; } = Array.Empty<Type>();

        internal static MobTransitionResult MakeFailure(IDictionary<Type, FailureTypes> typeToFailureTypesMap)
            => new MobTransitionResult { Success = false, TypeToFailureTypesMap = typeToFailureTypesMap };
    }
}
