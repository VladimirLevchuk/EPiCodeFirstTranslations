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

        ITranslationContent GetTranslations(CultureInfo culture);
    }

    public interface ITranslationService<TTranslationContent> : ITranslationService
        where TTranslationContent : ITranslationContent
    {
        string Translate(Expression<Func<TTranslationContent, string>> translationPathExpression);
    }
}