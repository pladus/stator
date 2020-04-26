using StatorTests.TestData.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatorTests.TestData.Models.Entities
{
    public class User
    {
        public UserStatus Status { get; set; } = UserStatus.Active;
    }
}
