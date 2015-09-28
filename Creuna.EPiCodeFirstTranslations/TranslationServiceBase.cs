using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using Castle.Core.Internal;
using Creuna.EPiCodeFirstTranslations.KeyBuilder;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace Creuna.EPiCodeFirstTranslations
{
    public abstract class TranslationServiceBase<TTranslationContent> : ITranslationService<TTranslationContent>
    {
        private readonly Lazy<LocalizationService> _localizationService = new Lazy<LocalizationService>(ServiceLocator.Current.GetInstance<LocalizationService>);
        private readonly Lazy<ITranslationsKeyMapper> _translationKeyMapper = new Lazy<ITranslationsKeyMapper>(ServiceLocator.Current.GetInstance<ITranslationsKeyMapper>);
        private readonly Lazy<ITranslationKeyBuilder<TTranslationContent>> _translationKeyBuilder = new Lazy<ITranslationKeyBuilder<TTranslationContent>>(ServiceLocator.Current.GetInstance<ITranslationKeyBuilder<TTranslationContent>>);

        private readonly Lazy<TTranslationContent> _translations = new Lazy<TTranslationContent>(ServiceLocator.Current.GetInstance<TTranslationContent>);
        private readonly Lazy<IEnumRegistry> _enumRegistry = new Lazy<IEnumRegistry>(ServiceLocator.Current.GetInstance<IEnumRegistry>);

        protected virtual LocalizationService LocalizationService { get { return _localizationService.Value; } }

        protected virtual ITranslationsKeyMapper TranslationsKeyMapper { get { return _translationKeyMapper.Value; } }
        protected virtual ITranslationKeyBuilder<TTranslationContent> TranslationsKeyBuilder { get { return _translationKeyBuilder.Value; } }

        protected virtual IEnumRegistry EnumRegistry { get { return _enumRegistry.Value; } }

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
            var enumRegistration = EnumRegistry.TryGetEnumRegistration(item.GetType());
            
            if (enumRegistration == null)
            {
                throw new InvalidOperationException("This type of enum is not registered to be translated.");
            }

            return TranslationsKeyMapper.GetTranslationKey(enumRegistration.EnumType, item.ToString(), enumRegistration.Alias);
        }

        public virtual IEnumerable<EnumRegistration> GetTranslatableEnumTypeRegistrations()
        {
            return EnumRegistry.GetTranslatableEnumTypeRegistrations();
        }

        [Obsolete("Inject EnumRegistry instead")]
        public virtual void RegisterEnumsAsTranslatable(IEnumerable<EnumRegistration> registrations)
        {
            foreach (var registration in registrations)
            {
                RegisterEnumAsTranslatable(registration);
            }
        }

        public virtual void RegisterEnumAsTranslatable(EnumRegistration registration)
        {
            EnumRegistry.RegisterEnumAsTranslatable(registration);
        }

        public virtual string GetTranslationKey(Expression<Func<TTranslationContent, string>> translationPath)
        {
            var key = TranslationsKeyBuilder.GetTranslationKey(translationPath);
            return key;
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