using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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

            if (!basicCulture.Equals(targetCulture))
            {
                return null;
            }

            string cacheKey = string.Format("contentHachCode:{0} key:{1}", translationContent.GetHashCode(), translationValueKey);
            var translation = _cache.GetOrLoad(cacheKey, () => ReadTranslation(translationContent, translationValueKey) ?? NullValue);

            return translation == NullValue ? null : translation;
        }

        public virtual string GetEnumTranslation(Type enumType, string translationValueKey, CultureInfo basicCulture, CultureInfo targetCulture)
        {
            if (enumType == null) throw new ArgumentNullException("enumType");
            if (!enumType.IsEnum) throw new ArgumentException("Unsupported type", "translationValueKey");
            if (basicCulture == null) throw new ArgumentNullException("basicCulture");
            if (targetCulture == null) throw new ArgumentNullException("targetCulture");

            if (!basicCulture.Equals(targetCulture))
            {
                return null;
            }

            string cacheKey = string.Format("enumType:{0} key:{1}", enumType.FullName, translationValueKey);
            var translation = _cache.GetOrLoad(cacheKey, () => ReadEnumTranslation(enumType, translationValueKey) ?? NullValue);

            return translation == NullValue ? null : translation;
        }

        protected virtual string ReadTranslation(object instance, string translationKey)
        {
            string[] propertyNames = translationKey.Split('.');

            foreach (var propertyName in propertyNames)
            {
                if (instance == null)
                {
                    return null;
                }

                var property = instance.GetType().GetProperty(propertyName);
                instance = property.GetValue(instance, null);
            }

            return (string)instance;
        }

        protected virtual string ReadEnumTranslation(Type enumType, string valueKey)
        {
            var valueField = enumType.GetField(valueKey);

            var displayAttr = (DisplayAttribute)Attribute.GetCustomAttribute(valueField, typeof(DisplayAttribute));
            if (displayAttr != null)
            {
                return displayAttr.Name;
            }

            return valueField.GetValue(null).ToString();
        }
    }
}