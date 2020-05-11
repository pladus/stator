using Stator;
using Stator.Interfaces;
using StatorTests.TestData.Enums;
using StatorTests.TestData.Models.Entities;
using StatorTests.TestData.Models.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace StatorTests
{
    public class StatorTransitionsTests
    {
        private readonly Stator<User, UserStatus> _stator;
        public StatorTransitionsTests()
        {
            _stator = Stator<User, UserStatus>.InitStator()
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
        }

        public static IEnumerable<object[]> Should_allow_valid_transition_data_source()
        {
            yield return new object[] { new UserBlockedEvent(), UserStatus.Active, UserStatus.Inactive };
            yield return new object[] { new UserBlockedEvent(), UserStatus.Striked, UserStatus.Deleted };
            yield return new object[] { new UserBlockedEvent(), UserStatus.Premium, UserStatus.Inactive };
            yield return new object[] { new UserStarredEvent(), UserStatus.Active, UserStatus.Premium };
        }

        public static IEnumerable<object[]> Should_disallow_valid_transition_data_source()
        {
            yield return new object[] { new UserBlockedEvent(), UserStatus.Deleted };
            yield return new object[] { new UserBlockedEvent(), UserStatus.Inactive };
            yield return new object[] { new UserStarredEvent(), UserStatus.Premium };
            yield return new object[] { new UserStarredEvent(), UserStatus.Deleted };
            yield return new object[] { new UserStarredEvent(), UserStatus.Inactive };
            yield return new object[] { new UserStarredEvent(), UserStatus.Striked };
        }

        [Theory]
        [MemberData(nameof(Should_allow_valid_transition_data_source))]
        public void Should_allow_valid_transition(IEvent<User> @event, UserStatus statusFrom, UserStatus statusTo)
        {
            var user = new User { Status = statusFrom };
            var result = _stator.Go(user, @event);

            Assert.Equal(statusTo, user.Status);
            Assert.Equal(user, result.Entity);
            Assert.True(result.Success);
        }

        [Theory]
        [MemberData(nameof(Should_disallow_valid_transition_data_source))]
        public void Should_disallow_valid_transition(IEvent<User> @event, UserStatus status)
        {
            var user = new User { Status = status };
            var result = _stator.Go(user, @event);

            Assert.Equal(status, user.Status);
            Assert.Equal(user, result.Entity);
            Assert.False(result.Success);
        }
    }
}
