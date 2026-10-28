using System.Reflection;
using NHibernate.PersistenceTesting.Utils;
using NUnit.Framework;

namespace Tests.Testing
{
    [TestFixture]
    public class MemberSpecs
    {
        [TestCase("PublicProperty", false, false, true, false)]
        [TestCase("PrivateProperty", true, false, false, false)]
        [TestCase("ProtectedProperty", false, true, false, false)]
        [TestCase("InternalProperty", false, false, false, true)]
        [TestCase("WriteOnlyProperty", false, false, false, false)]
        public void should_report_getter_visibility_without_requiring_a_getter(
            string propertyName, bool isPrivate, bool isProtected, bool isPublic, bool isInternal)
        {
            var property = typeof(PropertyEntity).GetProperty(propertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var member = property.ToMember();

            Assert.That(member.IsPrivate, Is.EqualTo(isPrivate));
            Assert.That(member.IsProtected, Is.EqualTo(isProtected));
            Assert.That(member.IsPublic, Is.EqualTo(isPublic));
            Assert.That(member.IsInternal, Is.EqualTo(isInternal));
        }

        private class PropertyEntity
        {
            public string? PublicProperty { get; set; }
            private string? PrivateProperty { get; set; }
            protected string? ProtectedProperty { get; set; }
            internal string? InternalProperty { get; set; }
            public string? WriteOnlyProperty { set { } }
        }
    }
}
