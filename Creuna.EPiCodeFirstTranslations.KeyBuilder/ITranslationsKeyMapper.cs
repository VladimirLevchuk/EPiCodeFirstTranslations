using System;
using System.Collections.Generic;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    public interface ITranslationsKeyMapper
    {
        string GetPropertyPathOrEnumValueKey(Type translationContentType, string translationKey, string alias = null);

        string GetTranslationKey(Type translationContentType, string propertyPath, string alias = null);

        Dictionary<string, string> QueryTranslationKeyToPropertyPathMap(Type translationContentType, string propertyPath, string alias = null);
    }
}