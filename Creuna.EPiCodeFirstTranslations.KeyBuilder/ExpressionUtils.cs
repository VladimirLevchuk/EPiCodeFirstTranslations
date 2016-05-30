using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;

namespace Creuna.EPiCodeFirstTranslations
{
    /// <summary>
    /// Contains the utility methods that help to get required information form LINQ expression instance.
    /// </summary>
    public static class ExpressionUtils
    {
        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> instance of defined by LINQ expression property.
        /// </summary>
        /// <typeparam name="T">The property owner type.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>
        /// The specified <see cref="PropertyInfo"/> instance.
        /// </returns>
        public static PropertyInfo GetPropertyInfo<T>([NotNull] Expression<Func<T, object>> propertyExpression)
        {
            return GetPropertyInfo<T, object>(propertyExpression);
        }

        /// <summary>
        /// Gets the <see cref="PropertyInfo"/> instance of defined by LINQ expression property.
        /// </summary>
        /// <typeparam name="T">The property owner type.</typeparam>
        /// <typeparam name="TValue">The type of the property value.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>
        /// The specified <see cref="PropertyInfo"/> instance.
        /// </returns>
        public static PropertyInfo GetPropertyInfo<T, TValue>([NotNull] Expression<Func<T, TValue>> propertyExpression)
        {
            const string propertyExpressionParamName = "propertyExpression";

            if (propertyExpression == null)
            {
                throw new ArgumentNullException(propertyExpressionParamName);
            }

            PropertyInfo property = null;

            Expression body = propertyExpression.Body;
            if (body.NodeType == ExpressionType.Convert || body.NodeType == ExpressionType.ConvertChecked)
            {
                body = ((UnaryExpression)body).Operand;
            }

            MemberExpression mex = body as MemberExpression;
            if (mex != null && mex.Expression.NodeType == ExpressionType.Parameter)
            {
                property = mex.Member as PropertyInfo;
            }

            if (property == null)
            {
                throw new ArgumentException("Unable to get property info from expression.", propertyExpressionParamName);
            }

            return property;
        }

        [NotNull]
        public static PropertyInfo GetPropertyInfo([NotNull] Type type, [NotNull] IEnumerable<string> path)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (path == null) throw new ArgumentNullException(nameof(path));

            PropertyInfo property = null;
            var currentType = type;
            var currentPath = new StringBuilder(type.FullName);

            foreach (var currentProperty in path)
            {
                currentPath.AppendFormat(".{0}", currentProperty);
                property = currentType.GetProperty(currentProperty);

                if (property == null)
                {
                    throw new InvalidOperationException(
                        $"Property not found, Path: '{currentPath}', current type: '{currentType}'");
                }

                currentType = property.PropertyType;
            }

            if (property == null)
            {
                throw new ArgumentException("Path shouldn't be empty", nameof(path));
            }

            return property;
        }

        /// <summary>
        /// Converts the defined LINQ expression to <see cref="string"/> representation of the path to the specified property.
        /// </summary>
        /// <typeparam name="T">The root property owner type.</typeparam>
        /// <param name="propertyPathExpression">The property path expression.</param>
        /// <returns>
        /// The path to the specified property.
        /// </returns>
        public static string GetPropertyPath<T>([NotNull] Expression<Func<T, object>> propertyPathExpression)
        {
            return GetPropertyPathString<T, object>(propertyPathExpression);
        }

        /// <summary>
        /// Converts the defined LINQ expression to <see cref="string"/> representation of the path to the specified property.
        /// </summary>
        /// <typeparam name="T">The root property owner type.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="propertyPathExpression">The property path expression.</param>
        /// <returns>
        /// The path to the specified property.
        /// </returns>
        public static string GetPropertyPathString<T, TValue>([NotNull] Expression<Func<T, TValue>> propertyPathExpression)
        {
            var path = GetPropertyPath(propertyPathExpression);
            var result = string.Join(".", path.ToArray());
            return result;
        }


        public static string GetPropertyPathString(IEnumerable<string> propertyPath)
        {
            var result = string.Join(".", propertyPath.ToArray());
            return result;
        }

        public static IEnumerable<string> GetPropertyPath<T, TValue>([NotNull] Expression<Func<T, TValue>> propertyPathExpression)
        {
            const string propertyPathExpressionParamName = "propertyExpression";

            if (propertyPathExpression == null)
            {
                throw new ArgumentNullException(propertyPathExpressionParamName);
            }

            var stack = new Stack<string>();
            Expression body = propertyPathExpression.Body;
            if (body.NodeType == ExpressionType.Convert || body.NodeType == ExpressionType.ConvertChecked)
            {
                body = ((UnaryExpression)body).Operand;
            }

            MemberExpression mex = body as MemberExpression;
            while (mex != null)
            {
                stack.Push(mex.Member.Name);
                mex = mex.Expression as MemberExpression;
            }

            if (stack.Count == 0)
            {
                throw new ArgumentException("Unable to get property path from expression.", propertyPathExpressionParamName);
            }

            return stack;
        }
    }
}
