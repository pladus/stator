using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace StatorTests.TestData.Models.Entities
{
    public class State : IEquatable<State>
    {
        public string RawMode { get; set; }

        public bool Equals([AllowNull] State other)
        {
            return RawMode == other.RawMode;
        }

        public override int GetHashCode()
        {
            return RawMode.GetHashCode();
        }
    }
}
