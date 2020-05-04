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
    public class StatorResilienceTests
    {
        private readonly static Stator<Order, State> _stator;
        static StatorResilienceTests()
        {
            _stator = Stator<Order, State>.InitStator()
                .State(x => x.State)

                .ForEvent<OrderCreatedEvent>()
                    .SetTransition( default, new State { RawMode = "Created" }).ConfirmTransition()
                    .ConfirmEvent()
                    .Build();
        }

        [Fact]
        public void Should_be_resilient_against_unregistered_events() 
        {
            var order = new Order {  State = new State { RawMode= "Created" } };
            var @event = new OrderDeliveredEvent();

            var result = _stator.CommitTransition(order, @event);
            Assert.Equal(FailureTypes.EventNotRegistered, result.FailureType);
        }

        [Fact]
        public void Should_be_resilient_against_null_state()
        {
            var order = new Order();
            var @event = new OrderCreatedEvent();

            var result = _stator.CommitTransition(order, @event);
            Assert.True(result.Success);
        }
    }
}
