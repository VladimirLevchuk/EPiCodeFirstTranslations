using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.ServiceLocation;

namespace Creuna.EPiCodeFirstTranslations
{
    public class TranslationProvider
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

            var translationContentType = TranslationService.GetTranslationContentType();

            var translationKeysMap = TranslationsKeyMapper.GetValueKeysMap(translationContentType, translationKey);

            if (translationKeysMap.Count > 0)
            {
                var translationContent = TranslationService.GetTranslations(culture);
                if (translationContent != null)
                {
                    var values = new Dictionary<string, string>();
                    foreach (string key in translationKeysMap.Keys)
                    {
                        string value = TranslationReader.GetTranslation(translationContent, translationKeysMap[key], culture);
                        if (value != null)
                        {
                            values.Add(key, value);
                        }
                    }

                    return values;
                }
            }

            return new Dictionary<string, string>();
        }

        public virtual string GetTranslationValue(string translationKey, CultureInfo culture)
        {
            var translationContentType = TranslationService.GetTranslationContentType();
            var valueKey = TranslationsKeyMapper.GetValueKey(translationContentType, translationKey);
            if (!string.IsNullOrEmpty(valueKey))
            {
                var translationContent = TranslationService.GetTranslations(culture);
                if (translationContent != null)
                {
                    return TranslationReader.GetTranslation(translationContent, valueKey, culture);
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