using System;
using System.Collections.Generic;

namespace NHibernate.PersistenceTesting
{
    /// <summary>
    /// Provides an <see cref="EqualityComparer{T}"/> implementation for <see cref="DateTimeOffset"/> with allowance for a
    /// certain amount of error.
    /// </summary>
    public class DateTimeOffsetEqualityComparer : EqualityComparer<DateTimeOffset>
    {
        private readonly TimeSpan maxDifference;
        private readonly bool ignoreTimeZone;

        public DateTimeOffsetEqualityComparer(TimeSpan maxDifference)
        {
            this.maxDifference = maxDifference;
        }

        public DateTimeOffsetEqualityComparer(TimeSpan maxDifference, bool ignoreTimeZone)
        {
            this.maxDifference = maxDifference;
            this.ignoreTimeZone = ignoreTimeZone;
        }

        public override bool Equals(DateTimeOffset x, DateTimeOffset y)
        {
            var duration = this.ignoreTimeZone ? (x.DateTime - y.DateTime).Duration() : (x - y).Duration();
            return duration <= this.maxDifference;
        }

        public override int GetHashCode(DateTimeOffset obj)
        {
            return obj.GetHashCode();
        }
    }
}