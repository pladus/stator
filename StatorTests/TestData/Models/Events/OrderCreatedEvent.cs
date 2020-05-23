﻿using Stator.Interfaces;
using StatorTests.TestData.Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatorTests.TestData.Models.Events
{
    class OrderCreatedEvent : IEvent
    {
        public object[] Args => Array.Empty<object>();
    }
}
