using Stator.Interfaces;
using StatorTests.TestData.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatorTests.TestData.Models.Events
{
    class OrderDeliveredEvent : IEvent<Order>
    {
        public Order Item => null;
        public object[] Args => Array.Empty<object>();
    }
}
