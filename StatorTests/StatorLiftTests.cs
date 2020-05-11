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
    public class StatorLiftTests
    {
        private readonly static StatorEventLift<User, UserStatus> _statorBlockedLift;

        static StatorLiftTests()
        {
            var stator = Stator<User, UserStatus>.InitStator()
                .State(x => x.Status)

                .ForEvent<UserStarredEvent>()
                    .SetTransition(UserStatus.Active, UserStatus.Premium).ConfirmTransition()
                    .ConfirmEvent()

                .ForEvent<UserBlockedEvent>()
                    .SetTransition(UserStatus.Active, UserStatus.Inactive).ConfirmTransition()
                    .SetTransition(UserStatus.Premium, UserStatus.Inactive).ConfirmTransition()
                    .SetTransition(UserStatus.Striked, UserStatus.Deleted).ConfirmTransition()

                    .ConfirmEvent()

                    .Build();

            _statorBlockedLift = stator.GetEventLift(new UserBlockedEvent());
        }

        [Fact]
        public void Should_be_resilient_against_unregistered_events()
        {
            var stator = Stator<User, UserStatus>.InitStator()
                .State(x => x.Status)
                .Build();

            var lift = stator.GetEventLift(new UserDeletedEvent());
            var result = lift.Go(new User());

            Assert.False(result.Success);
            Assert.Equal(FailureTypes.EventNotRegistered, result.FailureType);
        }

        [Fact]
        public void Should_perform_correct_transition()
        {
            var user = new User { Status = UserStatus.Active };
            var userArray = new[]
            { 
                new User { Status = UserStatus.Striked },
                new User { Status = UserStatus.Premium },
                new User { Status = UserStatus.Inactive}
            };

            var resultSingle = _statorBlockedLift.Go(user);
            var resultGroup = _statorBlockedLift.Go(userArray);

            Assert.Equal(UserStatus.Inactive, resultSingle.Entity.Status);
            Assert.Equal(user, resultSingle.Entity);
            Assert.True(resultSingle.Success);

            var groupFailsCount =
                resultGroup
                .Where(x => x.Success == false && x.FailureType == FailureTypes.TransitionNotRegistered)
                .Count();

            Assert.Equal(1, groupFailsCount);
        }
    }
}
