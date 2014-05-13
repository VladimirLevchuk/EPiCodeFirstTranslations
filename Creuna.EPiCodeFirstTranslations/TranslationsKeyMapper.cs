using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Creuna.EPiCodeFirstTranslations.Attributes;

namespace Creuna.EPiCodeFirstTranslations
{
    public class TranslationsKeyMapper
    {
        private const string RootedKeyStart = "~/";
        private const string KeyPartsSeparator = "/";
        private const string PropertyPathSeparator = ".";

        private readonly Dictionary<Type, Dictionary<string, string>> _keyMaps = new Dictionary<Type, Dictionary<string, string>>();

        public Dictionary<string, string> GetValueKeysMap(Type translationContentType, string translationKey)
        {
            translationKey = PrepareTranslationKey(translationKey ?? string.Empty);
            var keysMap = GetTranslationKeysMap(translationContentType);

            if (string.IsNullOrEmpty(translationKey))
            {
                return keysMap;
            }

            keysMap = keysMap.Keys.Where(key => key.StartsWith(translationKey)).ToDictionary(key => key, key => keysMap[key]);
            return keysMap;
        }

        public string GetValueKey(Type translationContentType, string translationKey)
        {
            translationKey = PrepareTranslationKey(translationKey);
            var keysMap = GetTranslationKeysMap(translationContentType);
            string propertyKey;
            keysMap.TryGetValue(translationKey, out propertyKey);
            return propertyKey;
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

            if (!key.EndsWith(KeyPartsSeparator))
            {
                key = key + KeyPartsSeparator;
            }

            return key;
        }

        protected virtual Dictionary<string, string> GetTranslationKeysMap(Type translationContentType)
        {
            Dictionary<string, string> map;
            if (!_keyMaps.TryGetValue(translationContentType, out map))
            {
                map = BuildTranslationKeysMap(translationContentType);
                _keyMaps.Add(translationContentType, map);
            }

            return map;
        }

        protected virtual Dictionary<string, string> BuildTranslationKeysMap(Type contentType)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            FetchContentTranslationKeys(map, contentType, Enumerable.Empty<string>(), string.Empty);
            return map;
        }

        protected virtual void FetchContentTranslationKeys(Dictionary<string, string> map, Type contentType, IEnumerable<string> parentTranslationPaths, string contentTypePath)
        {
            var localContentTranslationPaths = GetTranslationPaths(contentType);
            var currentContentTranslationPaths = BuildKeyPaths(parentTranslationPaths, localContentTranslationPaths).ToList();

            var translationProps = GetTranslationProperties(contentType);
            foreach (var translationProp in translationProps)
            {
                var propertyTranslationKeys = GetTranslationPropertyKeys(translationProp);
                propertyTranslationKeys = BuildKeyPaths(currentContentTranslationPaths, propertyTranslationKeys);
                foreach (var propertyKey in propertyTranslationKeys)
                {
                    var translationKey = PrepareTranslationKey(propertyKey);
                    var propertyPath = CombineTypePropertyPath(contentTypePath, translationProp.Name);
                    map.Add(translationKey, propertyPath);
                }
            }

            var childContentTypeProps = GetChildTranslationContentTypeProperties(contentType);
            foreach (var childContentTypeProp in childContentTypeProps)
            {
                FetchContentTranslationKeys(map, childContentTypeProp.PropertyType, currentContentTranslationPaths, CombineTypePropertyPath(contentTypePath, childContentTypeProp.Name));
            }
        }

        protected virtual IEnumerable<string> GetTranslationPaths(Type contentType)
        {
            var paths = new HashSet<string>();

            var pathAttrs = contentType.GetCustomAttributes(typeof(TranslationPathAttribute), false).Cast<TranslationPathAttribute>();
            foreach (var pathAttr in pathAttrs)
            {
                paths.Add(pathAttr.Path);
            }

            if (paths.Count == 0)
            {
                var defaultPath = contentType.Name;
                paths.Add(defaultPath);
            }

            return paths;
        }

        protected virtual IEnumerable<string> BuildKeyPaths(IEnumerable<string> parentLevelPaths, IEnumerable<string> currentLevelPaths)
        {
            var parentLevelPathsList = parentLevelPaths as IList<string> ?? parentLevelPaths.ToList();
            var resultPaths = new List<string>();
            foreach (var currentLevelPath in currentLevelPaths)
            {
                if (IsPathRooted(currentLevelPath) || parentLevelPathsList.Count == 0)
                {
                    resultPaths.Add(currentLevelPath);
                }
                else
                {
                    resultPaths.AddRange(parentLevelPathsList.Select(parentLevelPath => CombineTranslationKeyParts(parentLevelPath, currentLevelPath)));
                }
            }

            return resultPaths;
        }

        protected virtual string CombineTranslationKeyParts(string left, string right)
        {
            bool leftSeparator = left.EndsWith(KeyPartsSeparator);
            bool rightSeparator = right.StartsWith(KeyPartsSeparator);
            if (leftSeparator && rightSeparator)
            {
                return left + right.Substring(KeyPartsSeparator.Length);
            }

            if (leftSeparator || rightSeparator)
            {
                return left + right;
            }

            return left + KeyPartsSeparator + right;
        }

        protected virtual string CombineTypePropertyPath(string parentPath, string propertyName)
        {
            if (string.IsNullOrEmpty(parentPath))
            {
                return propertyName;
            }

            return parentPath + PropertyPathSeparator + propertyName;
        }

        protected virtual bool IsPathRooted(string path)
        {
            return path.StartsWith(RootedKeyStart);
        }

        protected virtual IEnumerable<string> GetTranslationPropertyKeys(PropertyInfo propertyInfo)
        {
            var keys = new HashSet<string>();
            var defaultKey = propertyInfo.Name;
            keys.Add(defaultKey);
            var keyAttrs = propertyInfo.GetCustomAttributes(typeof(TranslationKeyAttribute), false).Cast<TranslationKeyAttribute>();
            foreach (var keyAttr in keyAttrs)
            {
                keys.Add(keyAttr.Key);
            }

            return keys;
        }

        protected virtual IEnumerable<PropertyInfo> GetChildTranslationContentTypeProperties(Type contentType)
        {
            var childContentProps = contentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.PropertyType.IsClass && p.PropertyType != typeof(CultureInfo));

            return childContentProps;
        }

        protected virtual IEnumerable<PropertyInfo> GetTranslationProperties(Type contentType)
        {
            return contentType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.PropertyType == typeof(string))
                .ToList();
        }
    }
}