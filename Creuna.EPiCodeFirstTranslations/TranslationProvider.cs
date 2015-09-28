using System;
using System.Collections.Generic;
using System.Globalization;
using Creuna.EPiCodeFirstTranslations.KeyBuilder;

namespace Creuna.EPiCodeFirstTranslations
{
    public interface ITranslationProvider
    {
        Dictionary<string, string> GetTranslationValues(string translationKey, CultureInfo culture);
        string GetTranslationValue(string translationKey, CultureInfo culture);
        IEnumerable<CultureInfo> GetSupportedLanguages();
    }

    public class TranslationProvider : ITranslationProvider
    {
        public TranslationProvider(ITranslationService translationService, TranslationsKeyMapper translationsKeyMapper, TranslationReader translationReader)
        {
            if (translationService == null)
            {
                throw new ArgumentNullException("translationService");
            }

            if (translationsKeyMapper == null)
            {
                throw new ArgumentNullException("translationsKeyMapper");
            }

            if (translationReader == null)
            {
                throw new ArgumentNullException("translationReader");
            }

            TranslationService = translationService;
            TranslationReader = translationReader;
            TranslationsKeyMapper = translationsKeyMapper;
        }

        protected virtual TranslationsKeyMapper TranslationsKeyMapper { get; private set; }

        protected virtual TranslationReader TranslationReader { get; private set; }

        protected virtual ITranslationService TranslationService { get; private set; }

        public virtual Dictionary<string, string> GetTranslationValues(string translationKey, CultureInfo culture)
        {
            if (TranslationService == null)
            {
                return null;
            }

            var values = new Dictionary<string, string>();

            var basicCulture = TranslationService.GetBasicCulture();
            var translationContentType = TranslationService.GetTranslationContentType();
            var translationKeysMap = TranslationsKeyMapper.QueryTranslationKeyToPropertyPathMap(translationContentType, translationKey);

            if (translationKeysMap.Count > 0)
            {
                var translationContent = TranslationService.GetTranslations(culture);
                if (translationContent != null)
                {
                    foreach (string key in translationKeysMap.Keys)
                    {
                        string value = TranslationReader.GetTranslation(translationContent, translationKeysMap[key], basicCulture, culture);
                        if (value != null)
                        {
                            values.Add(key, value);
                        }
                    }
                }
            }

            foreach (var enumRegistration in TranslationService.GetTranslatableEnumTypeRegistrations())
            {
                var enumTranslationKeysMap = TranslationsKeyMapper.QueryTranslationKeyToPropertyPathMap(enumRegistration.EnumType, translationKey, enumRegistration.Alias);
                foreach (string key in enumTranslationKeysMap.Keys)
                {
                    string value = TranslationReader.GetEnumTranslation(enumRegistration.EnumType, enumTranslationKeysMap[key], basicCulture, culture);
                    if (value != null)
                    {
                        values.Add(key, value);
                    }
                }
            }

            return values;
        }

        public virtual string GetTranslationValue(string translationKey, CultureInfo culture)
        {
            var basicCulture = TranslationService.GetBasicCulture();
            var translationContentType = TranslationService.GetTranslationContentType();
            var valueKey = TranslationsKeyMapper.GetPropertyPathOrEnumValueKey(translationContentType, translationKey);
            if (!string.IsNullOrEmpty(valueKey))
            {
                var translationContent = TranslationService.GetTranslations(culture);
                if (translationContent != null)
                {
                    return TranslationReader.GetTranslation(translationContent, valueKey, basicCulture, culture);
                }
            }

            foreach (var enumRegistation in TranslationService.GetTranslatableEnumTypeRegistrations())
            {
                valueKey = TranslationsKeyMapper.GetPropertyPathOrEnumValueKey(enumRegistation.EnumType, translationKey, enumRegistation.Alias);
                if (!string.IsNullOrEmpty(valueKey))
                {
                    return TranslationReader.GetEnumTranslation(enumRegistation.EnumType, valueKey, basicCulture, culture);
                }
            }

            return null;
        }

        public virtual IEnumerable<CultureInfo> GetSupportedLanguages()
        {
            return TranslationService.GetSupportedCultures();
        }
    }
}