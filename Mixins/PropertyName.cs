using System;
using System.Linq.Expressions;

namespace Mixins
{
    /// <summary>
    /// Gets property name using lambda expressions.
    /// </summary>
    public static class PropertyName
    {
        public static string For<T, T1>(Expression<Func<T, T1>> expression)
        {
            var body = expression.Body;
            return GetMemberName(body);
        }
		
        public static string For<T>(Expression<Func<T>> expression)
        {
            var body = expression.Body;
            return GetMemberName(body);
        }

        public static string GetMemberName(Expression expression)
        {
            var memberExpression = expression as MemberExpression;
            if (memberExpression != null)
            {
                if (memberExpression.Expression.NodeType == ExpressionType.MemberAccess)
                {
                    return GetMemberName(memberExpression.Expression) + "." + memberExpression.Member.Name;
                }
                return memberExpression.Member.Name;
            }

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null)
            {
                if (unaryExpression.NodeType != ExpressionType.Convert)
                    throw new Exception(string.Format("Cannot interpret member from {0}", expression));

                return GetMemberName(unaryExpression.Operand);
            }

            throw new Exception(string.Format("Could not determine member from {0}", expression));
        }
    }
}