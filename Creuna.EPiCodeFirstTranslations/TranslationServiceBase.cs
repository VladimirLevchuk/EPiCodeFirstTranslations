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
        private readonly Dictionary<Type, EnumRegistration> _translatableEnumRegistrations = new Dictionary<Type, EnumRegistration>();

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
            EnumRegistration enumRegistration = null;
            if (_translatableEnumRegistrations.TryGetValue(item.GetType(), out enumRegistration))
            {
                return TranslationsKeyMapper.GetTranslationKey(enumRegistration.EnumType, item.ToString(), enumRegistration.Alias);
            }
            else
            {
                throw new InvalidOperationException("This type of enum is not registered to be translated.");
            }

        }

        public virtual IEnumerable<EnumRegistration> GetTranslatableEnumTypeRegistrations()
        {
            return _translatableEnumRegistrations.Values;
        }

        public void RegisterEnumsAsTranslatable(IEnumerable<EnumRegistration> registrations)
        {
            // TODO: update for using following notation:
            /*registry.Add<Gender>();
            registry.Add<Position>();
            registry.Add<Enums2.Gender>("Gender 2");*/

            foreach (var registration in registrations)
            {
                RegisterEnumAsTranslatable(registration);
            }
        }

        public virtual void RegisterEnumAsTranslatable(EnumRegistration registration)
        {
            if (registration == null) throw new ArgumentNullException("registration");
            if (!registration.EnumType.IsEnum) throw new ArgumentException("Type is not enum.", "registration");

            if (_translatableEnumRegistrations.ContainsKey(registration.EnumType))
            {
                throw new ArgumentException("This type of enum has already been registered.");
            }

            if (_translatableEnumRegistrations.Values.Any(x => x.Alias.Equals(registration.Alias, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Enum with the same alias has already been registered.", "registration");
            }

            _translatableEnumRegistrations.Add(registration.EnumType, registration);
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