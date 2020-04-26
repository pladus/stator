using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Enums
{
    public enum FailureTypes
    {
        None = 0,
        EventNotRegistered = 1,
        TransitionNotRegistered = 2,
        TransitionConditionFailed = 3
    }
}
