using System;
using System.Collections.Generic;
using System.Text;

namespace Stator
{
    public class EntityGetSetAccessor
    {
        public Func<object, object, object> TransitionInvoker;
        public Action<object, object> SetInvoker;
        public Func<object, object> GetInvoker;
    }
}
