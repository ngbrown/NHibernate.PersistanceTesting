using System;
using System.Collections.Generic;

namespace NHibernate.PersistenceTesting
{
    /// <summary>
    /// Provides an <see cref="EqualityComparer{T}"/> implementation for <see cref="DateTime"/> with allowance for a
    /// certain amount of error.
    /// </summary>
    public class DateTimeEqualityComparer : EqualityComparer<DateTime>
    {
        private readonly TimeSpan maxDifference;

        public DateTimeEqualityComparer(TimeSpan maxDifference)
        {
            this.maxDifference = maxDifference;
        }

        public override bool Equals(DateTime x, DateTime y)
        {
            var duration = (x - y).Duration();
            return duration <= this.maxDifference;
        }

        public override int GetHashCode(DateTime obj)
        {
            return obj.GetHashCode();
        }
    }
}