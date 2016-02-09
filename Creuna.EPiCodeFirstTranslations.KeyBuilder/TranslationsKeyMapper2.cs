using System;
using System.Collections.Generic;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    /// <summary>
    /// in progress - a replacement for TranslationsKeyMapper to fix "it allows to have several properties of the same type" issue
    /// TranslationKey - a string key that can be used to get a translated string
    /// PropertyPath - a string path that can be used to find a property in the .net type
    /// </summary>
    public class TranslationsKeyMapper2 : ITranslationsKeyMapper
    {
        private const string RootedKeyStart = "~/";
        private const string KeyPartsSeparator = "/";
        private const string PropertyPathSeparator = ".";
        private const string CacheKeySeparator = "|";

        private readonly Dictionary<string, string> _cache = new Dictionary<string, string>();
        private readonly Dictionary<Type, Dictionary<string, string>> _translationKeyToPropertyPathMaps = new Dictionary<Type, Dictionary<string, string>>();

        public virtual string GetPropertyPathOrEnumValueKey(Type translationContentType, string translationKey, string alias = null)
        {
            throw new NotImplementedException();
        }

        public virtual string GetTranslationKey(Type translationContentType, string propertyPath, string alias = null)
        {
            var cacheKey = BuildCacheKey(translationContentType, propertyPath);
            if (_cache.ContainsKey(cacheKey))
            {
                var value = _cache[cacheKey];
                return value;
            }

            var result = BuildCacheAndReturnTranslationKey(translationContentType, propertyPath, alias);
            return result;
        }

        private string BuildCacheAndReturnTranslationKey(Type translationContentType, string propertyPath, string @alias)
        {
            throw new NotImplementedException();
        }

        public virtual Dictionary<string, string> QueryTranslationKeyToPropertyPathMap(Type translationContentType, string translationKey, string alias = null)
        {
            throw new NotImplementedException();
        }

        private string BuildCacheKey(Type translationContentType, string propertyPath)
        {
            return translationContentType.AssemblyQualifiedName + CacheKeySeparator + propertyPath;
        }

        protected virtual string PrepareTranslationKey(string key)
        {
            if (key.StartsWith(RootedKeyStart))
            {
                key = key.Substring(RootedKeyStart.Length);
            }

            if (!key.StartsWith(KeyPartsSeparator))
            {
                key = KeyPartsSeparator + key;
            }

            if (key.EndsWith(KeyPartsSeparator))
            {
                key = key.Substring(key.Length - KeyPartsSeparator.Length);
            }

            return key;
        }
    }
}