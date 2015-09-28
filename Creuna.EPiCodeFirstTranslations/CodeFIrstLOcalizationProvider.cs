using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace Creuna.EPiCodeFirstTranslations
{
    public class CodeFirstLocalizationProvider : LocalizationProvider
    {
        private readonly Lazy<ITranslationProvider> _translationResolver = new Lazy<ITranslationProvider>(ServiceLocator.Current.GetInstance<ITranslationProvider>);

        protected virtual ITranslationProvider TranslationProvider { get { return _translationResolver.Value; } }

        public override string GetString(string originalKey, string[] normalizedKey, CultureInfo culture)
        {
            return TranslationProvider.GetTranslationValue(originalKey, culture);
        }

        public override IEnumerable<ResourceItem> GetAllStrings(string originalKey, string[] normalizedKey, CultureInfo culture)
        {
            return TranslationProvider.GetTranslationValues(originalKey, culture).Select(i => new ResourceItem(i.Key, i.Value, culture));
        }

        public override IEnumerable<CultureInfo> AvailableLanguages
        {
            get { return TranslationProvider.GetSupportedLanguages(); }
        }
    }
}
