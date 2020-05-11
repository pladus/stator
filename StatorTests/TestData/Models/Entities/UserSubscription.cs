using StatorTests.TestData.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatorTests.TestData.Models.Entities
{
    public class UserSubscription
    {
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    }
}
