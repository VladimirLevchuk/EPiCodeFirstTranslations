using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    /// <summary>
    /// Builds translation keys for the code-first translations
    /// </summary>
    /// <typeparam name="TTranslationContent"></typeparam>
    public interface ITranslationKeyBuilder<TTranslationContent>
    {
        /// <summary>
        /// Builds translation key for the given (by an expression) code-first translation class property
        /// </summary>
        /// <param name="translationPath"></param>
        /// <returns></returns>
        string GetTranslationKey(Expression<Func<TTranslationContent, string>> translationPath);

        /// <summary>
        /// Builds translation key for the given property path
        /// </summary>
        /// <param name="propertyPath">example: { "Labels", "MyLabel" } for translations class property Labels.MyLabel  </param>
        /// <returns></returns>
        string GetTranslationKey(IEnumerable<string> propertyPath);

        /// <summary>
        /// Builds translation key for the given enum item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="alias"></param>
        /// <returns></returns>
        string GetEnumTranslationKey(Enum item, string alias = null);
    }
}