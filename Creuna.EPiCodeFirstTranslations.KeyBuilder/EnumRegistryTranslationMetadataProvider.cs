using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    public class EnumRegistryTranslationMetadataProvider : IEnumTranslationMetadataProvider
    {
        public IEnumRegistry EnumRegistry { get; }

        public EnumRegistryTranslationMetadataProvider(IEnumRegistry enumEnumRegistry)
        {
            EnumRegistry = enumEnumRegistry;
        }

        public virtual string GetEnumTranslationKey(Enum item, string alias = null)
        {
            var enumRegistration = EnumRegistry.TryGetEnumRegistration(item.GetType());

            if (enumRegistration == null)
            {
                throw new InvalidOperationException("This type of enum is not registered to be translated.");
            }

            var key = BuildEnumTranslationKey(item, alias ?? enumRegistration.Alias);
            return key;
        }

        protected virtual string BuildEnumTranslationKey(Enum item, [CanBeNull] string alias)
        {
            var typeKey = GetEnumTypeKey(item.GetType(), alias);
            var itemKey = GetEnumItemKey(item);
            var result = BuildEnumTranslationKey(typeKey, itemKey);
            return result;
        }

        protected virtual string GetEnumTypeKey(Type enumType, string alias)
        {
            var typeKey = alias ?? enumType.Name;
            return typeKey;
        }

        protected virtual string BuildEnumTranslationKey(FieldInfo enumItemInfo, [CanBeNull] string alias)
        {
            var typeKey = GetEnumTypeKey(enumItemInfo.DeclaringType, alias);
            var itemKey = GetEnumItemKey(enumItemInfo);
            var result = BuildEnumTranslationKey(typeKey, itemKey);
            return result;
        }

        protected virtual string GetEnumItemKey(FieldInfo enumItemInfo)
        {
            // TODO: [low] support TranslationEnum/TranslationEnumItemPath attributes
            return enumItemInfo.Name;
        }

        protected virtual string GetEnumItemKey(Enum item)
        {
            // TODO: [low] support TranslationEnum/TranslationEnumItemPath attributes
            return item.ToString();
        }

        protected virtual string BuildEnumTranslationKey([NotNull] string typeKey, [NotNull] string itemKey)
        {
            if (typeKey == null) throw new ArgumentNullException(nameof(typeKey));
            if (itemKey == null) throw new ArgumentNullException(nameof(itemKey));

            return $"/Enums/{typeKey}/{itemKey}";
        }

        public virtual IEnumerable<string> GetAllKeys()
        {
            var allKeys = new List<string>();

            foreach (var registration in EnumRegistry.GetTranslatableEnumTypeRegistrations())
            {
                foreach (var enumField in registration.EnumType.GetFields().Where(x => !x.IsSpecialName))
                {
                    allKeys.Add(BuildEnumTranslationKey(enumField, registration.Alias));
                }
            }

            return allKeys;
        }
    }
}