using Stator;
using Stator.Interfaces;
using StatorTests.TestData.Enums;
using StatorTests.TestData.Models.Entities;
using StatorTests.TestData.Models.Events;
using System;
using System.Collections.Generic;
using Xunit;

namespace StatorTests
{
    public class MobStatorTransitionsTests
    {
        private readonly Stator<User, UserStatus> _stator;

        public static IEnumerable<object[]> Should_allow_valid_transition_for_2_entities_data_source()
        {
            yield return new object[] { new UserBlockedEvent(), UserStatus.Active, UserStatus.Inactive, SubscriptionStatus.Active, SubscriptionStatus.Closed };
            yield return new object[] { new UserBlockedEvent(), UserStatus.Striked, UserStatus.Deleted, SubscriptionStatus.Active, SubscriptionStatus.Closed };
            yield return new object[] { new UserBlockedEvent(), UserStatus.Premium, UserStatus.Inactive, SubscriptionStatus.Pending, SubscriptionStatus.Closed };
        }

        [Theory]
        [MemberData(nameof(Should_allow_valid_transition_for_2_entities_data_source))]
        public void Should_allow_valid_transition_for_2_entities(
            IEvent @event,
            UserStatus userStatusFrom,
            UserStatus userStatusTo,
            SubscriptionStatus subscriptionStatusFrom,
            SubscriptionStatus subscriptionStatusTo)
        {
            var statorForUser = Stator<User, UserStatus>.InitStator()
            .State(x => x.Status)
            .ForEvent<UserBlockedEvent>()
                .SetTransition(UserStatus.Active, UserStatus.Inactive).ConfirmTransition()
                .SetTransition(UserStatus.Premium, UserStatus.Inactive).ConfirmTransition()
                .SetTransition(UserStatus.Striked, UserStatus.Deleted).ConfirmTransition()
                .ConfirmEvent()

             .Build();

            var statorForSubscription = Stator<UserSubscription, SubscriptionStatus>.InitStator()
            .State(x => x.Status)
            .ForEvent<UserBlockedEvent>()
                .SetTransition(SubscriptionStatus.Active, SubscriptionStatus.Closed).ConfirmTransition()
                .SetTransition(SubscriptionStatus.Pending, SubscriptionStatus.Closed).ConfirmTransition()
                .ConfirmEvent()

             .Build();

            var mobStator =
                MobStatorFactory
                .InitWithStator(statorForUser)
                .AddStateMachine(statorForSubscription)
                .Build();

            var user = new User { Status = userStatusFrom };
            var subscription = new UserSubscription { Status = subscriptionStatusFrom };
            var result = mobStator.Go(@event, Tuple.Create(user, subscription));

            Assert.Equal(userStatusTo, user.Status);
            Assert.Equal(subscriptionStatusTo, subscription.Status);
            Assert.True(result.Success);
        }
        [Fact]
        public void Should_allow_valid_transition_for_any_entities()
        {
            var statorForUser = Stator<User, UserStatus>.InitStator()
            .State(x => x.Status)
            .ForEvent<UserBlockedEvent>()
                .SetTransition(UserStatus.Active, UserStatus.Inactive).ConfirmTransition()
                .SetTransition(UserStatus.Premium, UserStatus.Inactive).ConfirmTransition()
                .SetTransition(UserStatus.Striked, UserStatus.Deleted).ConfirmTransition()
                .ConfirmEvent()
             .Build();

            var statorForSubscription = Stator<UserSubscription, SubscriptionStatus>.InitStator()
            .State(x => x.Status)
            .ForEvent<UserBlockedEvent>()
                .SetTransition(SubscriptionStatus.Active, SubscriptionStatus.Closed).ConfirmTransition()
                .SetTransition(SubscriptionStatus.Pending, SubscriptionStatus.Closed).ConfirmTransition()
                .ConfirmEvent()
             .Build();

            var statorForPersonalSpace = Stator<PersonalSpace, PersonalSpaceStatus>.InitStator()
                .State(x => x.Status)
                .ForEvent<UserBlockedEvent>()
                    .SetTransition(PersonalSpaceStatus.Granted, PersonalSpaceStatus.Blocked).ConfirmTransition()
                    .ConfirmEvent()
                 .Build();

            var statorForAccount = Stator<Account, AccountStatus>.InitStator()
                .State(x => x.Status)
                .ForEvent<UserBlockedEvent>()
                    .SetTransition(AccountStatus.Existed, AccountStatus.Deleted).ConfirmTransition()
                    .SetTransition(AccountStatus.Blocked, AccountStatus.Deleted).ConfirmTransition()
                    .ConfirmEvent()
                 .Build();

            var statorForRanking = Stator<Ranking, RankingStatus>.InitStator()
                .State(x => x.Status)
                .ForEvent<UserBlockedEvent>()
                    .SetTransition(RankingStatus.Tracked, RankingStatus.Untracked).ConfirmTransition()
                    .ConfirmEvent()
                 .Build();

            var mobStator =
                MobStatorFactory
                .InitWithStator(statorForUser)
                .AddStateMachine(statorForSubscription)
                .AddStateMachine(statorForAccount)
                .AddStateMachine(statorForPersonalSpace)
                .AddStateMachine(statorForRanking)
                .Build();

            var user = new User { Status = UserStatus.Active };
            var subscription = new UserSubscription { Status = SubscriptionStatus.Active };
            var space = new PersonalSpace { Status = PersonalSpaceStatus.Granted };
            var ranking = new Ranking { Status = RankingStatus.Tracked };
            var account = new Account { Status = AccountStatus.Blocked };
            var @event = new UserBlockedEvent();

            var result = mobStator.Go(@event, user, subscription, account, space, ranking);

            Assert.True(result.Success);
        }
    }
}
