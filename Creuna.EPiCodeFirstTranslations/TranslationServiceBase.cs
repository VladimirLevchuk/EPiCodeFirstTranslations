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

        protected virtual LocalizationService LocalizationService { get { return _localizationService.Value; } }

        public string Translate(Expression<Func<TTranslationContent, string>> translationPath)
        {
            if (translationPath == null)
            {
                throw new ArgumentNullException("translationPath");
            }

            return GetTranslation(GetTranslationKey(translationPath));
        }

        public Type GetTranslationContentType()
        {
            return ServiceLocator.Current.GetInstance<TTranslationContent>().GetType();
        }

        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            return new[] { ServiceLocator.Current.GetInstance<TTranslationContent>().ContentCulture };
        }

        public virtual ITranslationContent GetTranslations(CultureInfo culture)
        {
            return ServiceLocator.Current.GetInstance<TTranslationContent>();
        }

        protected virtual string GetTranslationKey(Expression<Func<TTranslationContent, string>> translationPath)
        {
            return "/" + ExpressionUtils.GetPropertyPath(translationPath).Replace(".", "/");
        }

        protected virtual string GetTranslation(string translationKey)
        {
            return LocalizationService.GetString(translationKey);
        }
    }
}