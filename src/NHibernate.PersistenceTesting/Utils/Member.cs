using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NHibernate.PersistenceTesting.Utils
{
    [Serializable]
    public abstract class Member : IEquatable<Member>
    {
        public abstract string Name { get; }
        public abstract System.Type PropertyType { get; }
        public abstract bool CanWrite { get; }
        public abstract MemberInfo MemberInfo { get; }
        public abstract System.Type DeclaringType { get; }
        public abstract bool HasIndexParameters { get; }
        public abstract bool IsMethod { get; }
        public abstract bool IsField { get; }
        public abstract bool IsProperty { get; }
        public abstract bool IsAutoProperty { get; }
        public abstract bool IsPrivate { get; }
        public abstract bool IsProtected { get; }
        public abstract bool IsPublic { get; }
        public abstract bool IsInternal { get; }

        public bool Equals(Member other)
        {
            return Equals(other.MemberInfo.MetadataToken, this.MemberInfo.MetadataToken) && Equals(other.MemberInfo.Module, this.MemberInfo.Module);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is Member)) return false;
            return this.Equals((Member)obj);
        }

        public override int GetHashCode()
        {
            return this.MemberInfo.GetHashCode() ^ 3;
        }

        public static bool operator ==(Member left, Member right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Member left, Member right)
        {
            return !Equals(left, right);
        }

        public abstract void SetValue(object target, object value);
        public abstract object GetValue(object target);
    }

    [Serializable]
    internal class MethodMember : Member
    {
        private readonly MethodInfo member;

        public override void SetValue(object target, object value)
        {
            throw new NotSupportedException("Cannot set the value of a method Member.");
        }

        public override object GetValue(object target)
        {
            return this.member.Invoke(target, null);
        }

        public MethodMember(MethodInfo member)
        {
            this.member = member;
        }

        public override string Name
        {
            get { return this.member.Name; }
        }
        public override System.Type PropertyType
        {
            get { return this.member.ReturnType; }
        }
        public override bool CanWrite
        {
            get { return false; }
        }
        public override MemberInfo MemberInfo
        {
            get { return this.member; }
        }
        public override System.Type DeclaringType
        {
            get { return this.member.DeclaringType; }
        }
        public override bool HasIndexParameters
        {
            get { return false; }
        }
        public override bool IsMethod
        {
            get { return true; }
        }
        public override bool IsField
        {
            get { return false; }
        }
        public override bool IsProperty
        {
            get { return false; }
        }

        public override bool IsAutoProperty
        {
            get { return false; }
        }

        public override bool IsPrivate
        {
            get { return this.member.IsPrivate; }
        }

        public override bool IsProtected
        {
            get { return this.member.IsFamily || this.member.IsFamilyAndAssembly; }
        }

        public override bool IsPublic
        {
            get { return this.member.IsPublic; }
        }

        public override bool IsInternal
        {
            get { return this.member.IsAssembly || this.member.IsFamilyAndAssembly; }
        }

        public bool IsCompilerGenerated
        {
            get { return this.member.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Any(); }
        }

        public override string ToString()
        {
            return "{Method: " + this.member.Name + "}";
        }
    }

    [Serializable]
    internal class FieldMember : Member
    {
        private readonly FieldInfo member;

        public override void SetValue(object target, object value)
        {
            this.member.SetValue(target, value);
        }

        public override object GetValue(object target)
        {
            return this.member.GetValue(target);
        }

        public FieldMember(FieldInfo member)
        {
            this.member = member;
        }

        public override string Name
        {
            get { return this.member.Name; }
        }
        public override System.Type PropertyType
        {
            get { return this.member.FieldType; }
        }
        public override bool CanWrite
        {
            get { return true; }
        }
        public override MemberInfo MemberInfo
        {
            get { return this.member; }
        }
        public override System.Type DeclaringType
        {
            get { return this.member.DeclaringType; }
        }
        public override bool HasIndexParameters
        {
            get { return false; }
        }
        public override bool IsMethod
        {
            get { return false; }
        }
        public override bool IsField
        {
            get { return true; }
        }
        public override bool IsProperty
        {
            get { return false; }
        }

        public override bool IsAutoProperty
        {
            get { return false; }
        }

        public override bool IsPrivate
        {
            get { return this.member.IsPrivate; }
        }

        public override bool IsProtected
        {
            get { return this.member.IsFamily || this.member.IsFamilyAndAssembly; }
        }

        public override bool IsPublic
        {
            get { return this.member.IsPublic; }
        }

        public override bool IsInternal
        {
            get { return this.member.IsAssembly || this.member.IsFamilyAndAssembly; }
        }

        public override string ToString()
        {
            return "{Field: " + this.member.Name + "}";
        }
    }

    [Serializable]
    internal class PropertyMember : Member
    {
        readonly PropertyInfo member;
        readonly MethodMember getMethod;
        readonly MethodMember setMethod;

        public PropertyMember(PropertyInfo member)
        {
            this.member = member;
            this.getMethod = this.GetMember(member.GetGetMethod(true));
            this.setMethod = this.GetMember(member.GetSetMethod(true));
        }

        MethodMember GetMember(MethodInfo method)
        {
            if (method == null)
                return null;

            return (MethodMember)method.ToMember();
        }

        public override void SetValue(object target, object value)
        {
            this.member.SetValue(target, value, null);
        }

        public override object GetValue(object target)
        {
            return this.member.GetValue(target, null);
        }

        public override string Name
        {
            get { return this.member.Name; }
        }
        public override System.Type PropertyType
        {
            get { return this.member.PropertyType; }
        }
        public override bool CanWrite
        {
            get
            {
                // override the default reflection value here. Private setters aren't
                // considered "settable" in the same sense that public ones are. We can
                // use this to control the access strategy later
                if (this.IsAutoProperty && (this.setMethod == null || this.setMethod.IsPrivate))
                    return false;

                return this.member.CanWrite;
            }
        }
        public override MemberInfo MemberInfo
        {
            get { return this.member; }
        }
        public override System.Type DeclaringType
        {
            get { return this.member.DeclaringType; }
        }
        public override bool HasIndexParameters
        {
            get { return this.member.GetIndexParameters().Length > 0; }
        }
        public override bool IsMethod
        {
            get { return false; }
        }
        public override bool IsField
        {
            get { return false; }
        }
        public override bool IsProperty
        {
            get { return true; }
        }

        public override bool IsAutoProperty
        {
            get
            {
                return (this.getMethod != null && this.getMethod.IsCompilerGenerated) 
                    || (this.setMethod != null && this.setMethod.IsCompilerGenerated);
            }
        }

        public override bool IsPrivate
        {
            get { return this.getMethod.IsPrivate; }
        }

        public override bool IsProtected
        {
            get { return this.getMethod.IsProtected; }
        }

        public override bool IsPublic
        {
            get { return this.getMethod.IsPublic; }
        }

        public override bool IsInternal
        {
            get { return this.getMethod.IsInternal; }
        }

        public MethodMember Get
        {
            get { return this.getMethod; }
        }

        public MethodMember Set
        {
            get { return this.setMethod; }
        }

        public override string ToString()
        {
            return "{Property: " + this.member.Name + "}";
        }
    }

    public static class MemberExtensions
    {
        public static Member ToMember(this PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                throw new NullReferenceException("Cannot create member from null.");
            
            return new PropertyMember(propertyInfo);
        }

        public static Member ToMember(this MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new NullReferenceException("Cannot create member from null.");

            return new MethodMember(methodInfo);
        }

        public static Member ToMember(this FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                throw new NullReferenceException("Cannot create member from null.");

            return new FieldMember(fieldInfo);
        }

        public static Member ToMember(this MemberInfo memberInfo)
        {
            if (memberInfo == null)
                throw new NullReferenceException("Cannot create member from null.");

            if (memberInfo is PropertyInfo)
                return ((PropertyInfo)memberInfo).ToMember();
            if (memberInfo is FieldInfo)
                return ((FieldInfo)memberInfo).ToMember();
            if (memberInfo is MethodInfo)
                return ((MethodInfo)memberInfo).ToMember();

            throw new InvalidOperationException("Cannot convert MemberInfo '" + memberInfo.Name + "' to Member.");
        }
    }
}
