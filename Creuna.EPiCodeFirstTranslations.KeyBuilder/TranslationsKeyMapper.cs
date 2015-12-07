using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Creuna.EPiCodeFirstTranslations.KeyBuilder.Annotation;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{

    public class TranslationsKeyMapper : ITranslationsKeyMapper
    {
        private const string RootedKeyStart = "~/";
        private const string KeyPartsSeparator = "/";
        private const string PropertyPathSeparator = ".";

        private readonly Dictionary<Type, Dictionary<string, string>> _propertyPathToTranslationKeyMaps = new Dictionary<Type, Dictionary<string, string>>();
        private readonly Dictionary<Type, Dictionary<string, string>> _translationKeyToPropertyPathMaps = new Dictionary<Type, Dictionary<string, string>>();

        public event EventHandler<DuplicateKeyEventArg> DuplicateKeyFound;

        public virtual Dictionary<string, string> QueryTranslationKeyToPropertyPathMap(Type translationContentType, string translationKey, string translationContentAlias = null)
        {
            translationKey = PrepareTranslationKey(translationKey ?? string.Empty);
            var keysMap = GetTranslationKeyToPropertyPathMap(translationContentType, translationContentAlias);

            if (string.IsNullOrEmpty(translationKey))
            {
                return keysMap;
            }

            keysMap = keysMap.Keys.Where(key => key.StartsWith(translationKey)).ToDictionary(key => key, key => keysMap[key]);
            return keysMap;
        }

        public virtual string GetPropertyPathOrEnumValueKey(Type translationContentType, string translationKey, string translationContentAlias = null)
        {
            translationKey = PrepareTranslationKey(translationKey);
            var valueMap = GetTranslationKeyToPropertyPathMap(translationContentType, translationContentAlias);
            string propertyKey;
            valueMap.TryGetValue(translationKey, out propertyKey);
            return propertyKey;
        }

        public string GetTranslationKey(Type translationContentType, string valueKey, string translationContentAlias = null)
        {
            var keyMap = GetPropertyPathToTranslationKeyMap(translationContentType, translationContentAlias);
            string key;
            keyMap.TryGetValue(valueKey, out key);
            return key;
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

        protected virtual Dictionary<string, string> GetTranslationKeyToPropertyPathMap(Type translationContentType, string translationContentAlias)
        {
            Dictionary<string, string> map;
            while (!_translationKeyToPropertyPathMaps.TryGetValue(translationContentType, out map))
            {
                BuildTranslationKeysMaps(translationContentType, translationContentAlias);
            }

            return map;
        }

        protected virtual Dictionary<string, string> GetPropertyPathToTranslationKeyMap(Type translationContentType, string translationContentAlias)
        {
            Dictionary<string, string> map;
            while (!_propertyPathToTranslationKeyMaps.TryGetValue(translationContentType, out map))
            {
                BuildTranslationKeysMaps(translationContentType, translationContentAlias);
            }

            return map;
        }

        protected virtual void BuildTranslationKeysMaps(Type contentType, string contentAlias)
        {
            var translationKeyToPropertyPathMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var propertyPathToTranslationKeyMap = new Dictionary<string, string>();

            if (contentType.IsEnum)
            {
                FetchEnumTranslationKeys(translationKeyToPropertyPathMap, propertyPathToTranslationKeyMap, contentType, contentAlias);
            }
            else
            {
                List<String> rootTranslationPaths = new List<string>();
                if (!string.IsNullOrEmpty(contentAlias))
                {
                    rootTranslationPaths.Add(contentAlias);
                }

                FetchContentTranslationKeys(translationKeyToPropertyPathMap, propertyPathToTranslationKeyMap, contentType, rootTranslationPaths, string.Empty);
            }

            _translationKeyToPropertyPathMaps.Add(contentType, translationKeyToPropertyPathMap);
            _propertyPathToTranslationKeyMaps.Add(contentType, propertyPathToTranslationKeyMap);
        }

        protected virtual void FetchEnumTranslationKeys(Dictionary<string, string> translationKeyToPropertyPathMap, Dictionary<string, string> propertyPathToTranslationKeyMap, Type enumType, string enumTypeAlias)
        {
            var enumFields = enumType.GetFields();
            foreach (var enumField in enumFields)
            {
                if (!enumField.IsSpecialName)
                {
                    string propertyPath = enumField.Name;
                    string translationKey = string.Format("/Enums/{0}/{1}", enumTypeAlias ?? enumType.Name, propertyPath);
                    translationKeyToPropertyPathMap.Add(translationKey, propertyPath);
                    propertyPathToTranslationKeyMap.Add(propertyPath, translationKey);
                }
            }
        }

        protected virtual void FetchContentTranslationKeys(Dictionary<string, string> translationKeyToPropertyPathMap, 
            Dictionary<string, string> propertyPathToTranslationKeyMap, 
            Type contentType, 
            IEnumerable<string> parentTranslationPaths, 
            string contentTypePath)
        {
            if (contentType.IsPrimitive || contentType == typeof (string))
            {
                return;
            }

            // var processedProperties = new HashSet<string>();

            var localContentTranslationPaths = GetTranslationPaths(contentType);
            var currentContentTranslationPaths = BuildKeyPaths(parentTranslationPaths, localContentTranslationPaths).ToList();

            var translationProps = GetTranslationProperties(contentType).ToList();
            // processedProperties.Add(translationProps.Select(x => x.PropertyType.FullName, ));

            foreach (var translationProp in translationProps)
            {
                var propertyTranslationKeys = GetTranslationPropertyKeys(translationProp);
                var keyList = BuildKeyPaths(currentContentTranslationPaths, propertyTranslationKeys).ToList();
                var propertyPath = CombineTypePropertyPath(contentTypePath, translationProp.Name);

                foreach (var propertyKey in keyList)
                {
                    var translationKey = PrepareTranslationKey(propertyKey);
                    
                    if (translationKeyToPropertyPathMap.ContainsKey(translationKey))
                    {
                        var e = new DuplicateKeyEventArg
                        {
                            TranslationKeyToPropertyPathMap = translationKeyToPropertyPathMap.OrEmpty(),
                            PropertyPathToTranslationKeyMap = propertyPathToTranslationKeyMap.OrEmpty(),
                            TranslationKey = translationKey,
                            PropertyKey = propertyKey
                        };
                        OnDuplicateKeyFound(e);
                    }
                    translationKeyToPropertyPathMap[translationKey] = propertyPath;
                }

                if (keyList.Count > 0)
                {
                    propertyPathToTranslationKeyMap[propertyPath] = PrepareTranslationKey(keyList.First());
                }
            }

            var childContentTypeProps = GetChildTranslationContentTypeProperties(contentType);
            foreach (var childContentTypeProp in childContentTypeProps)
            {
                //if (processedProperties.Contains(childContentTypeProp.PropertyType))
                //    continue;

                //processedProperties.Add(childContentTypeProp.PropertyType);

                FetchContentTranslationKeys(translationKeyToPropertyPathMap, propertyPathToTranslationKeyMap, 
                    childContentTypeProp.PropertyType, currentContentTranslationPaths, 
                    CombineTypePropertyPath(contentTypePath, childContentTypeProp.Name));
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
            var keyAttrs = propertyInfo.GetCustomAttributes(typeof(AlsoTranslationForKeyAttribute), false).Cast<AlsoTranslationForKeyAttribute>();
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

        protected virtual void OnDuplicateKeyFound(DuplicateKeyEventArg e)
        {
            var handler = DuplicateKeyFound;
            if (handler != null) handler(this, e);
        }
    }
}