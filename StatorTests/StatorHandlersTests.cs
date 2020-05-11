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
    public class StatorHandlersTests
    {
        [Fact]
        public void Should_invoke_transition_handlers()
        {
            var handlerBeforeTransition1Fired = false;
            var handlerAfterTransition1Fired = false;
            var handlerBeforeTransition2Fired = false;
            var handlerAfterTransition2Fired = false;

            var stator = Stator<User, UserStatus>.InitStator()
                .State(x => x.Status)
                .ForEvent<UserBlockedEvent>()
                    .SetTransition(UserStatus.Active, UserStatus.Inactive)
                        .WithActionBeforeTransition((entity, e) => handlerBeforeTransition1Fired = true)
                        .WithActionAfterTransition((entity, e) => handlerAfterTransition1Fired = true)
                        .ConfirmTransition()
                    .ConfirmEvent()
                    .Build();

            var @event = new UserBlockedEvent();
            var user = new User { Status = UserStatus.Active };
            var result = stator.Go(user, @event);

            Assert.True(handlerBeforeTransition1Fired);
            Assert.False(handlerBeforeTransition2Fired);
            Assert.True(handlerAfterTransition1Fired);
            Assert.False(handlerAfterTransition2Fired);
        }

        [Fact]
        public void Should_invoke_transition_miss_handlers()
        {
            var exMessage = "TM";

            var stator = Stator<User, UserStatus>.InitStator()
                .State(x => x.Status)
                .ForEvent<UserBlockedEvent>()
                    .WithTransitionMissHandler((_,__) =>  throw new Exception(exMessage))
                    .SetTransition(UserStatus.Active, UserStatus.Inactive)
                        .ConfirmTransition()
                    .ConfirmEvent()
                    .Build();

            var @event = new UserBlockedEvent();
            var user = new User { Status = UserStatus.Deleted };
            
            var ex = Assert.Throws<Exception>(() => stator.Go(user, @event));
            Assert.Equal(exMessage, ex.Message);
        }
        [Fact]
        public void Should_check_condition_and_invoke_condition_mismatch_handlers()
        {
            var exMessage = "CF";

            var stator = Stator<User, UserStatus>.InitStator()
                .State(x => x.Status)
                .ForEvent<UserBlockedEvent>()
                    .SetTransition(UserStatus.Active, UserStatus.Inactive)
                        .Match((u, e) => e.Args?.Length > 0)
                        .Or((_, __) => throw new Exception(exMessage))
                        .ConfirmTransition()
                    .ConfirmEvent()
                    .Build();

            var @event = new UserBlockedEvent();
            var user = new User { Status = UserStatus.Active };

            var ex = Assert.Throws<Exception>(() => stator.Go(user, @event));
            Assert.Equal(exMessage, ex.Message);
        }
    }
}
