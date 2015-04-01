using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Castle.Core.Internal;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace Creuna.EPiCodeFirstTranslations
{
    public abstract class TranslationServiceBase<TTranslationContent> : ITranslationService<TTranslationContent>
    {
        private readonly Lazy<LocalizationService> _localizationService = new Lazy<LocalizationService>(ServiceLocator.Current.GetInstance<LocalizationService>);
        private readonly Lazy<TranslationsKeyMapper> _translationKeyMapper = new Lazy<TranslationsKeyMapper>(ServiceLocator.Current.GetInstance<TranslationsKeyMapper>);
        private readonly Lazy<TTranslationContent> _translations = new Lazy<TTranslationContent>(ServiceLocator.Current.GetInstance<TTranslationContent>);

        protected virtual LocalizationService LocalizationService { get { return _localizationService.Value; } }

        protected virtual TranslationsKeyMapper TranslationsKeyMapper { get { return _translationKeyMapper.Value; } }

        public virtual string Translate(Expression<Func<TTranslationContent, string>> translationPath)
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

        public virtual string Translate(Expression<Func<TTranslationContent, string>> translationPath, string fallback)
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

            return GetTranslation(translationKey, fallback);
        }

        public virtual string TranslateEnumValue(Enum value)
        {
            string translationKey = GetEnumTranslationKey(value);
            var translation = GetTranslation(translationKey, string.Empty);
            if (translation.IsNullOrEmpty())
            {
                translation = GetTranslation(translationKey, value.ToString(), GetBasicCulture());
            }

            return translation;
        }

        public virtual Type GetTranslationContentType()
        {
            return _translations.Value.GetType();
        }

        public virtual IEnumerable<CultureInfo> GetSupportedCultures()
        {
            return new[] { GetBasicCulture() };
        }

        public virtual CultureInfo GetBasicCulture()
        {
            return CultureInfo.GetCultureInfo("en");
        }

        public virtual object GetTranslations(CultureInfo culture)
        {
            return _translations.Value;
        }

        public virtual string GetEnumTranslationKey(Enum item)
        {
            return TranslationsKeyMapper.GetTranslationKey(item.GetType(), item.ToString());
        }

        public virtual IEnumerable<Type> GetTranslatableEnumTypes()
        {
            return Enumerable.Empty<Type>();
        }

        protected virtual string GetTranslationKey(Expression<Func<TTranslationContent, string>> translationPath)
        {
            return TranslationsKeyMapper.GetTranslationKey(typeof(TTranslationContent), ExpressionUtils.GetPropertyPath(translationPath));
        }

        protected virtual string GetTranslation(string translationKey)
        {
            return LocalizationService.GetString(translationKey);
        }

        protected virtual string GetTranslation(string translationKey, string fallback)
        {
            return LocalizationService.GetString(translationKey, fallback);
        }

        protected virtual string GetTranslation(string translationKey, string fallback, CultureInfo culture)
        {
            return LocalizationService.GetStringByCulture(translationKey, fallback, culture);
        }
    }
}