using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace Creuna.EPiCodeFirstTranslations
{
    public interface ITranslationService
    {
        Type GetTranslationContentType();

        IEnumerable<CultureInfo> GetSupportedCultures();

        CultureInfo GetBasicCulture();

        object GetTranslations(CultureInfo culture);

        IEnumerable<EnumRegistration> GetTranslatableEnumTypeRegistrations();
    }

    public interface ITranslationService<TTranslationContent> : ITranslationService
    {
        string Translate(Expression<Func<TTranslationContent, string>> translationPathExpression);

        string Translate(Expression<Func<TTranslationContent, string>> translationPathExpression, string fallback);

        string TranslateEnumValue(Enum value);

        string GetTranslationKey(Expression<Func<TTranslationContent, string>> translationPath);

        string GetEnumTranslationKey(Enum item);
    }
}