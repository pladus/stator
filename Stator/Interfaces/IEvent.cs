using System;
using System.Collections.Generic;
using System.Text;

namespace Stator.Interfaces
{
    public interface IEvent<TEntity> where TEntity: class
    {
        TEntity Item { get; }
        object[] Args { get; }
    }
}
