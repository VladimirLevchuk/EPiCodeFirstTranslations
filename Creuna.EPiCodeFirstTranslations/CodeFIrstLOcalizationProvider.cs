using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace Creuna.EPiCodeFirstTranslations
{
    public class CodeFirstLocalizationProvider : LocalizationProvider
    {
        private readonly Lazy<TranslationProvider> _translationResolver = new Lazy<TranslationProvider>(ServiceLocator.Current.GetInstance<TranslationProvider>);

        protected virtual TranslationProvider TranslationProvider { get { return _translationResolver.Value; } }

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
