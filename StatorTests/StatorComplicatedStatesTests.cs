using Stator;
using Stator.Enums;
using Stator.Interfaces;
using StatorTests.TestData.Enums;
using StatorTests.TestData.Models.Entities;
using StatorTests.TestData.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace StatorTests
{
    public class StatorComplicatedStatesTests
    {
        private readonly static Stator<Order, State> _stator;
        static StatorComplicatedStatesTests()
        {
            _stator = Stator<Order, State>.InitStator()
                .State(x => x.State)
                .ForEvent<OrderCreatedEvent>()
                    .SetTransition(new State { RawMode = "Created" }, new State { RawMode = "Delivered" }).ConfirmTransition()
                .ConfirmEvent()
                .Build();
        }
        [Fact]
        public void Should_process_reference_type_states() 
        {
            var order = new Order {  State = new State { RawMode= "Created" } };
            var @event = new OrderCreatedEvent();

            var result = _stator.CommitTransition(order, @event);
            Assert.True(result.Success);
            Assert.Equal("Delivered", result.Entity.State.RawMode);
        }
    }
}
