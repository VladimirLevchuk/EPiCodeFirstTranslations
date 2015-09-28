using System;
using System.Linq.Expressions;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    public interface ITranslationKeyBuilder<TTranslationContent>
    {
        string GetTranslationKey(Expression<Func<TTranslationContent, string>> translationPath);

        string GetEnumTranslationKey(Enum item, string alias = null);
    }
}