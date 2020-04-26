﻿using Stator.Interfaces;
using StatorTests.TestData.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatorTests.TestData.Models.Events
{
    public class UserDeletedEvent : IEvent<User>
    {
        public User Item => null;

        public object Args => Array.Empty<object>();
    }
}