using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Creuna.EPiCodeFirstTranslations.KeyBuilder.Annotation;
using JetBrains.Annotations;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    public class TranslationAnnotationMetadataProvider : ITranslationMetadataProvider
    {
        public virtual string GetTranslationKey(PropertyInfo propertyInfo)
        {
            var pathAttribute = propertyInfo.GetFirstOrDefault<TranslationPathAttribute>();

            if (pathAttribute == null)
            {
                var propertyTypePathAttribute = propertyInfo.DeclaringType?.GetFirstOrDefault<TranslationPathAttribute>();

                if (propertyTypePathAttribute != null)
                {
                    return TranslationPath.Combine(propertyTypePathAttribute.Path, propertyInfo.Name);
                }

                return propertyInfo.Name;
            }

            return pathAttribute.Path;
        }

        public virtual string GetTranslationKey([NotNull] Type translationsType,
            [NotNull] IEnumerable<string> propertyPath)
        {
            if (translationsType == null) throw new ArgumentNullException(nameof(translationsType));
            if (propertyPath == null) throw new ArgumentNullException(nameof(propertyPath));
            var propertyInfo = ExpressionUtils.GetPropertyInfo(translationsType, propertyPath);
            return GetTranslationKey(propertyInfo);
        }

        public virtual IEnumerable<string> GetAllKeys(Type translationsType)
        {
            RecursionDepth = 0;
            var translationsRootPath = GetTranslationsRootPath(translationsType);
            var result = GetAllKeys(translationsType, translationsRootPath);
            return result;
        }

        private int RecursionDepth;
        const int MaxRecursionDepth = 100;

        protected virtual IEnumerable<string> GetAllKeys(Type translationsType, string currentPath)
        {
            ++RecursionDepth;

            if (RecursionDepth > MaxRecursionDepth)
            {
                throw new InvalidOperationException("Recursion depth limit exceeded. Check your translations type for reference loops. ");
            }

            // TODO: [low] handle cases with multiple keys per one member

            var allKeys = new List<string>();

            foreach (var propertyInfo in translationsType.GetProperties())
            {
                var propertyType = propertyInfo.PropertyType;
                var propertyTranslationPath = TranslationPath.Combine(currentPath, GetTranslationKey(propertyInfo));

                if (propertyType == typeof (string))
                {
                    allKeys.Add(propertyTranslationPath);

                    var otherKeys = propertyInfo.GetCustomAttributes<AlsoTranslationForKeyAttribute>().Select(x => x.Key).ToList();

                    allKeys.AddRange(otherKeys);
                }
                else
                {
                    var childTranslationPath = propertyTranslationPath;
                    var childKeys = GetAllKeys(propertyType, childTranslationPath);
                    allKeys.AddRange(childKeys);
                }
            }

            --RecursionDepth;
            return allKeys;
        }

        public virtual string GetTranslationsRootPath(Type translationsType)
        {
            var pathAttribute = translationsType.GetFirstOrDefault<TranslationPathAttribute>();
            
            var result = pathAttribute?.Path ?? "/" + translationsType.Name;

            return result;
        }
    }

    public static class ReflectionExtensions
    {
        public static TAttribute GetFirstOrDefaultSelfOrInherited<TAttribute>(this Type type)
            where TAttribute : Attribute
        {
            return type.GetFirstOrDefault<TAttribute>(true);
        }

        public static TAttribute GetFirstOrDefaultSelfOrInherited<TAttribute>(this MemberInfo memberInfo)
            where TAttribute : Attribute
        {
            return memberInfo.GetFirstOrDefault<TAttribute>(true);
        }


        public static TAttribute GetFirstOrDefault<TAttribute>(this Type type, bool inherit = false)
            where TAttribute: Attribute
        {
            var result = type.GetCustomAttributes(typeof(TAttribute),
                inherit).OfType<TAttribute>().FirstOrDefault();

            return result;
        }

        public static TAttribute GetFirstOrDefault<TAttribute>(this MemberInfo memberInfo, bool inherit = false)
            where TAttribute : Attribute
        {
            var result = memberInfo.GetCustomAttributes(typeof(TAttribute),
                inherit).OfType<TAttribute>().FirstOrDefault();

            return result;
        }

        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo memberInfo, bool inherit = false)
            where TAttribute : Attribute
        {
            var result = memberInfo.GetCustomAttributes(typeof(TAttribute),
                inherit).OfType<TAttribute>();

            return result;
        }
    }
}