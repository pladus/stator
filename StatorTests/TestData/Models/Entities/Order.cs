using System;
using System.Collections.Generic;
using System.Text;

namespace StatorTests.TestData.Models.Entities
{
    public class Order
    {
        public State State { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
