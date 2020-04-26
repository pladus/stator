using Stator;
using StatorTests.TestData.Enums;
using StatorTests.TestData.Models.Entities;
using StatorTests.TestData.Models.Events;
using System;
using System.Collections.ObjectModel;
using Xunit;

namespace StatorTests
{
    public class StatorTests
    {
        [Fact]
        public void ShouldConstructed()
        {
            var stator =
                Stator<User, UserStatus>.InitStator()
                .Status(x => x.Status)

                .ForEvent<UserStarredEvent>()
                    .SetTransition(UserStatus.Active, UserStatus.Premium).ConfirmTransition()                    
                    .ConfirmEvent()

                .ForEvent<UserBlockedEvent>()
                    .SetTransition(UserStatus.Active, UserStatus.Inactive).ConfirmTransition()
                    .SetTransition(UserStatus.Premium, UserStatus.Inactive).ConfirmTransition()
                    .ConfirmEvent()
                
                    .Build();

            var user = new User { Status = UserStatus.Active };
            var @event = new UserBlockedEvent();

            stator.CommitTransition(user, @event);

            Assert.Equal(UserStatus.Inactive, user.Status);
        }
    }
}
