using System;
using System.Globalization;
using System.Runtime.Caching;
using Creuna.EPiCodeFirstTranslations.Utils;

namespace Creuna.EPiCodeFirstTranslations
{
    public class TranslationReader
    {
        private readonly SimpleMemoryCache _cache = new SimpleMemoryCache(typeof(TranslationReader).FullName);

        public virtual string GetTranslation(ITranslationContent translationContent, string translationValueKey, CultureInfo culture)
        {
            if (translationContent == null)
            {
                throw new ArgumentNullException("translationContent");
            }

            if (string.IsNullOrEmpty(translationValueKey))
            {
                throw new ArgumentNullException("translationValueKey");
            }

            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }

            if (!culture.Equals(translationContent.ContentCulture))
            {
                return null;
            }

            string cacheKey = string.Format("contentHachCode:{0} key:{1}", translationContent.GetHashCode(), translationValueKey);
            var translation = _cache.GetOrLoad(cacheKey, () => ReadTranslation(translationContent, translationValueKey));

            return translation;
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

            return instance != null ? instance.ToString() : string.Empty;
        }
    }
}