using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Creuna.EPiCodeFirstTranslations.Attributes;
using Creuna.EPiCodeFirstTranslations.Utils;

namespace Creuna.EPiCodeFirstTranslations
{
    public class TranslationReader
    {
        private const string NullValue = "<NULL_CACHE_VALUE>";

        private readonly SimpleMemoryCache _cache = new SimpleMemoryCache(typeof(TranslationReader).FullName);

        public virtual string GetTranslation(object translationContent, string translationValueKey, CultureInfo basicCulture, CultureInfo targetCulture)
        {
            if (translationContent == null) throw new ArgumentNullException("translationContent");
            if (string.IsNullOrEmpty(translationValueKey)) throw new ArgumentNullException("translationValueKey");
            if (basicCulture == null) throw new ArgumentNullException("basicCulture");
            if (targetCulture == null) throw new ArgumentNullException("targetCulture");

            string cacheKey = string.Format("contentHachCode:{0} key:{1} culture{2}", translationContent.GetHashCode(), translationValueKey, targetCulture.Name);
            var translation = _cache.GetOrLoad(cacheKey, () => ReadTranslation(translationContent, translationValueKey, basicCulture, targetCulture) ?? NullValue);

            return translation == NullValue ? null : translation;
        }

        public virtual string GetEnumTranslation(Type enumType, string translationValueKey, CultureInfo basicCulture, CultureInfo targetCulture)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new ArgumentException("Unsupported type", "translationValueKey");
            if (basicCulture == null) throw new ArgumentNullException("basicCulture");
            if (targetCulture == null) throw new ArgumentNullException("targetCulture");


            string cacheKey = string.Format("enumType:{0} key:{1} culture{2}", enumType.FullName, translationValueKey, targetCulture.Name);
            var translation = _cache.GetOrLoad(cacheKey, () => ReadEnumTranslation(enumType, translationValueKey, basicCulture, targetCulture) ?? NullValue);

            return translation == NullValue ? null : translation;
        }

        protected virtual string ReadTranslation(object instance, string translationKey, CultureInfo basicCulture, CultureInfo targetCulture)
        {
            string[] propertyNames = translationKey.Split('.');
            var translationObject = instance;
            PropertyInfo translationProperty = null;
            int propertyNameIndex = 0;

            do
            {
                var propertyName = propertyNames[propertyNameIndex];
                translationProperty = translationObject.GetType().GetProperty(propertyName);

                if (propertyNameIndex == propertyNames.Length - 1)
                {
                    break;
                }

                translationObject = translationProperty.GetValue(translationObject, null);

                propertyNameIndex++;
            } while (translationObject != null);


            if (translationObject == null)
            {
                return null;
            }

            string translation = null;
            if (basicCulture.Equals(targetCulture))
            {
                translation = (string)translationProperty.GetValue(translationObject, null);
            }
            else
            {
                translation = GetCustomCultureTranslation(translationProperty, targetCulture);
            }

            return translation;
        }

        string ReadEnumTranslation(Type enumType, string valueKey, CultureInfo basicCulture, CultureInfo targetCulture)
        {
            var valueField = enumType.GetField(valueKey);

            if (basicCulture.Equals(targetCulture))
            {
                var displayAttr = (DisplayAttribute) Attribute.GetCustomAttribute(valueField, typeof (DisplayAttribute));
                if (displayAttr != null)
                {
                    return displayAttr.Name;
                }
            }
            else
            {
                return GetCustomCultureTranslation(valueField, targetCulture);
            }

            return valueField.GetValue(null).ToString();
        }

        private string GetCustomCultureTranslation(MemberInfo memberInfo, CultureInfo targetCulture)
        {
            var translationAttr = Attribute.GetCustomAttributes(memberInfo, typeof(TranslationForCultureAttribute))
                   .Cast<TranslationForCultureAttribute>()
                   .FirstOrDefault(a => a.Culture.Equals(targetCulture));

            if (translationAttr != null)
            {
                return translationAttr.Translation;
            }

            return null;
        }
    }
}