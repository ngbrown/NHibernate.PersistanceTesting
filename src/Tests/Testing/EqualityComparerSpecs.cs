using System;
using System.Collections.Generic;
using NHibernate.PersistenceTesting;
using NUnit.Framework;

namespace Tests.Testing
{
    public class With_DateTimeEqualityComparer_specification : Specification
    {
        protected DateTime originalDateTime;
        protected EqualityComparer<DateTime> equalityComparer;

        public override void establish_context()
        {
            originalDateTime = DateTime.Now;
        }
    }

    [TestFixture]
    public class When_the_date_time_is_truncated_to_second : With_DateTimeEqualityComparer_specification
    {
        private DateTime retreivedDateTime;

        public override void because()
        {
            retreivedDateTime = new DateTime(originalDateTime.Year, originalDateTime.Month, originalDateTime.Day, originalDateTime.Hour, originalDateTime.Minute, originalDateTime.Second);
            equalityComparer = new DateTimeEqualityComparer(TimeSpan.FromSeconds(1));
        }

        [Test]
        public void should_have_same_second()
        {
            retreivedDateTime.Second.ShouldEqual(originalDateTime.Second);
        }

        [Test]
        public void should_directly_compare_as_different()
        {
            retreivedDateTime.ShouldNotEqual(originalDateTime);
        }

        [Test]
        public void should_compare_as_same_with_equality_comparer()
        {
            equalityComparer.Equals(retreivedDateTime, originalDateTime).ShouldBeTrue();
        }
    }

    public class With_DateTimeOffsetEqualityComparer_specification : Specification
    {
        protected DateTimeOffset originalDateTime;
        protected EqualityComparer<DateTimeOffset> equalityComparer;

        public override void establish_context()
        {
            originalDateTime = DateTimeOffset.Now;
        }
    }

    [TestFixture]
    public class When_the_date_time_offset_is_truncated_to_second : With_DateTimeOffsetEqualityComparer_specification
    {
        private DateTimeOffset retreivedDateTime;

        public override void because()
        {
            retreivedDateTime = new DateTimeOffset(originalDateTime.Year, originalDateTime.Month, originalDateTime.Day, originalDateTime.Hour, originalDateTime.Minute, originalDateTime.Second, originalDateTime.Offset);
            equalityComparer = new DateTimeOffsetEqualityComparer(TimeSpan.FromSeconds(1));
        }

        [Test]
        public void should_have_same_second()
        {
            retreivedDateTime.Second.ShouldEqual(originalDateTime.Second);
        }

        [Test]
        public void should_directly_compare_as_different()
        {
            retreivedDateTime.ShouldNotEqual(originalDateTime);
        }

        [Test]
        public void should_compare_as_same_with_equality_comparer()
        {
            equalityComparer.Equals(retreivedDateTime, originalDateTime).ShouldBeTrue();
        }
    }
}