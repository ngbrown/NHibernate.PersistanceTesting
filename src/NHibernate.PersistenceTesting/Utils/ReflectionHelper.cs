using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NHibernate.PersistenceTesting.Utils
{
    public static class ReflectionHelper
    {
        public static Accessor GetAccessor<MODEL>(Expression<Func<MODEL, object>> expression)
        {
            MemberExpression memberExpression = GetMemberExpression(expression.Body);

            return getAccessor(memberExpression);
        }

        public static Accessor GetAccessor<MODEL, T>(Expression<Func<MODEL, T>> expression)
        {
            MemberExpression memberExpression = GetMemberExpression(expression.Body);

            return getAccessor(memberExpression);
        }

        public static Accessor GetAccessor<T>(Expression<Func<T>> expression)
        {
            MemberExpression memberExpression = GetMemberExpression(expression.Body);

            return getAccessor(memberExpression);
        }

        private static MemberExpression GetMemberExpression(Expression expression)
        {
            return GetMemberExpression(expression, true);
        }

        private static MemberExpression GetMemberExpression(Expression expression, bool enforceCheck)
        {
            MemberExpression memberExpression = null;
            if (expression.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression) expression;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression as MemberExpression;
            }

            if (enforceCheck && memberExpression == null)
            {
                throw new ArgumentException("Not a member access", "expression");
            }

            return memberExpression;
        }

        private static Accessor getAccessor(MemberExpression memberExpression)
        {
            var list = new List<Member>();

            while (memberExpression != null)
            {
                list.Add(memberExpression.Member.ToMember());
                memberExpression = memberExpression.Expression as MemberExpression;
            }

            if (list[list.Count - 1].IsField)
            {
                list.RemoveAt(list.Count - 1);
            }

            if (list.Count == 1)
            {
                return new SingleMember(list[0]);
            }

            list.Reverse();
            return new PropertyChain(list.ToArray());
        }
    }
}
