using Stator;
using Stator.Exceptions;
using Stator.Interfaces;
using StatorTests.TestData.Enums;
using StatorTests.TestData.Models.Entities;
using StatorTests.TestData.Models.Events;
using System;
using System.Collections.Generic;
using Xunit;

namespace StatorTests
{
    public class MobStatorRollbackTests
    {

        [Fact]
        public void Should_do_rollback_for_2_entities()
         {
            IMobStator<User, UserSubscription> mobStator = GetBrokenMobStatorForTwoEntities(true);

            var user = new User { Status = UserStatus.Active };
            var subscription = new UserSubscription { Status = SubscriptionStatus.Active };
            var @event = new UserBlockedEvent();
            Assert.Throws<Exception>(() => mobStator.Go(@event, Tuple.Create(user, subscription)));
            Assert.Equal(UserStatus.Active, user.Status);
            Assert.Equal(SubscriptionStatus.Active, subscription.Status);
        }
        [Fact]
        public void Should_not_do_rollback_for_2_entities()
        {
            IMobStator<User, UserSubscription> mobStator = GetBrokenMobStatorForTwoEntities(false);

            var user = new User { Status = UserStatus.Active };
            var subscription = new UserSubscription { Status = SubscriptionStatus.Active };
            var @event = new UserBlockedEvent();
            Assert.Throws<Exception>(() => mobStator.Go(@event, Tuple.Create(user, subscription)));
            Assert.NotEqual(UserStatus.Active, user.Status);
            Assert.NotEqual(SubscriptionStatus.Active, subscription.Status);
        }
        [Fact]
        public void Should_not_do_rollback_for_5_entities()
        {
            IMobStator mobStator = GetBrokenMobStatorWithFiveEntities(false);

            var user = new User { Status = UserStatus.Striked };
            var subscription = new UserSubscription { Status = SubscriptionStatus.Active };
            var space = new PersonalSpace { Status = PersonalSpaceStatus.Granted };
            var ranking = new Ranking { Status = RankingStatus.Tracked };
            var account = new Account { Status = AccountStatus.Blocked };
            var @event = new UserBlockedEvent();

            Assert.Throws<Exception>(() => mobStator.Go(@event, user, subscription, space, ranking, account));

            Assert.NotEqual(UserStatus.Striked, user.Status);
            Assert.NotEqual(SubscriptionStatus.Active, subscription.Status);
            Assert.NotEqual(PersonalSpaceStatus.Granted, space.Status);
            Assert.Equal(RankingStatus.Tracked, ranking.Status);
            Assert.Equal(AccountStatus.Blocked, account.Status);
        }
        [Fact]
        public void Should_do_rollback_for_5_entities()
        {
            IMobStator mobStator = GetBrokenMobStatorWithFiveEntities(true);

            var user = new User { Status = UserStatus.Striked };
            var subscription = new UserSubscription { Status = SubscriptionStatus.Active };
            var space = new PersonalSpace { Status = PersonalSpaceStatus.Granted };
            var ranking = new Ranking { Status = RankingStatus.Tracked };
            var account = new Account { Status = AccountStatus.Blocked };
            var @event = new UserBlockedEvent();

            Assert.Throws<Exception>(() => mobStator.Go(@event, user, subscription, account, space, ranking));

            Assert.Equal(UserStatus.Striked, user.Status);
            Assert.Equal(SubscriptionStatus.Active, subscription.Status);
            Assert.Equal(PersonalSpaceStatus.Granted, space.Status);
            Assert.Equal(RankingStatus.Tracked, ranking.Status);
            Assert.Equal(AccountStatus.Blocked, account.Status);
        }

        private static IMobStator GetBrokenMobStatorWithFiveEntities(bool withRollbackOnFailure)
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
                    .SetTransition(PersonalSpaceStatus.Granted, PersonalSpaceStatus.Blocked)
                    .WithActionAfterTransition((x, y) => throw new Exception())
                    .ConfirmTransition()
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
                .InitWithStator(statorForUser, withRollbackOnFailure)
                .AddStateMachine(statorForSubscription)
                .AddStateMachine(statorForAccount)
                .AddStateMachine(statorForPersonalSpace)
                .AddStateMachine(statorForRanking)
                .Build();
            return mobStator;
        }
        private static IMobStator<User, UserSubscription> GetBrokenMobStatorForTwoEntities(bool withRollbackOnFailure)
        {
            var statorForUser = Stator<User, UserStatus>.InitStator()
                .State(x => x.Status)
                .ForEvent<UserBlockedEvent>()
                    .SetTransition(UserStatus.Active, UserStatus.Inactive).ConfirmTransition()
                .ConfirmEvent()
                .Build();

            var statorForSubscription = Stator<UserSubscription, SubscriptionStatus>.InitStator()
                .State(x => x.Status)
                .ForEvent<UserBlockedEvent>()
                    .SetTransition(SubscriptionStatus.Active, SubscriptionStatus.Closed)
                    .WithActionAfterTransition((x,y) => throw new Exception())
                    .ConfirmTransition()
                .ConfirmEvent()
                .Build();

            var mobStator =
                MobStatorFactory
                .InitWithStator(statorForUser, withRollbackOnFailure)
                .AddStateMachine(statorForSubscription)
                .Build();

            return mobStator;
        }
    }
}
