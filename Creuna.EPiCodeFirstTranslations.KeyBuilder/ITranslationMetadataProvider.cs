using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    /// <summary>
    /// Provides metadata for translations class. 
    /// </summary>
    public interface ITranslationMetadataProvider
    {
        /// <summary>
        /// Returns path of the translation type to be used as a root for all contained key. </summary>
        /// <param name="translationsType"></param>
        /// <returns></returns>
        string GetTranslationsRootPath(Type translationsType);
        /// <summary>
        /// Returns property translation path where currentPath is a path of a 
        /// </summary>
        /// <param name="propertyInfo">Pass translation PropertyInfo if you have one, or leave it empty and implementation will find it automatically</param>
        /// <returns></returns>
        string GetTranslationKey(PropertyInfo propertyInfo = null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="translationsType"></param>
        /// <param name="propertyPath"></param>
        /// <returns></returns>
        string GetTranslationKey(Type translationsType, IEnumerable<string> propertyPath);
        /// <summary>
        /// Returns all keys defined in the give translation type (without culture). 
        /// </summary>
        /// <param name="translationsType"></param>
        /// <returns></returns>
        IEnumerable<string> GetAllKeys(Type translationsType);

        // TODO: [normal] handle alternative cultures
        // IEnumerable<string> GetKeysForAlternativeCulture(Type translationsType, CultureInfo culture);
    }
}