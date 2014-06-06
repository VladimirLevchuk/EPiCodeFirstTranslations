using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace Creuna.EPiCodeFirstTranslations
{
    public abstract class TranslationServiceBase<TTranslationContent> : ITranslationService<TTranslationContent>
        where TTranslationContent : ITranslationContent
    {
        private readonly Lazy<LocalizationService> _localizationService = new Lazy<LocalizationService>(ServiceLocator.Current.GetInstance<LocalizationService>);
        private readonly Lazy<TranslationsKeyMapper> _translationKeyMapper = new Lazy<TranslationsKeyMapper>(ServiceLocator.Current.GetInstance<TranslationsKeyMapper>);
        private readonly Lazy<TTranslationContent> _translations = new Lazy<TTranslationContent>(ServiceLocator.Current.GetInstance<TTranslationContent>); 

        protected virtual LocalizationService LocalizationService { get { return _localizationService.Value; } }

        protected virtual TranslationsKeyMapper TranslationsKeyMapper { get { return _translationKeyMapper.Value; } }

        public string Translate(Expression<Func<TTranslationContent, string>> translationPath)
        {
            if (translationPath == null)
            {
                throw new ArgumentNullException("translationPath");
            }

            var translationKey = GetTranslationKey(translationPath);
            if (translationKey == null)
            {
                throw new ArgumentException(string.Format("Unable to resolve translation by {0} expression.", translationPath), "translationPath");
            }

            return GetTranslation(translationKey);
        }

        public Type GetTranslationContentType()
        {
            return _translations.Value.GetType();
        }

        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            return new[] { _translations.Value.ContentCulture };
        }

        public virtual ITranslationContent GetTranslations(CultureInfo culture)
        {
            return _translations.Value;
        }

        protected virtual string GetTranslationKey(Expression<Func<TTranslationContent, string>> translationPath)
        {
            return TranslationsKeyMapper.GetTranslationKey(typeof(TTranslationContent), ExpressionUtils.GetPropertyPath(translationPath));
        }

        protected virtual string GetTranslation(string translationKey)
        {
            return LocalizationService.GetString(translationKey);
        }
    }
}