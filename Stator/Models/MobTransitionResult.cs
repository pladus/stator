using Stator.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Models
{
    public class MobTransitionResult
    {
        public bool Success { get; private set; } = true;
        public IDictionary<Type, FailureTypes> TypeToFailureTypesMap { get; private set; }

        internal static MobTransitionResult MakeFailure(IDictionary<Type, FailureTypes> typeToFailureTypesMap)
            => new MobTransitionResult { Success = false, TypeToFailureTypesMap = typeToFailureTypesMap };
    }
}
