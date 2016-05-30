using System;
using System.Collections.Generic;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    /// <summary>
    /// Provides translations metadata for enums
    /// </summary>
    public interface IEnumTranslationMetadataProvider
    {
        /// <summary>
        /// Returns enum item translation key
        /// </summary>
        /// <param name="item"></param>
        /// <param name="alias">Alias can be used to distiguish enums with the same name from different namespaces, 
        /// by default enum translation path is built using its name</param>
        /// <returns></returns>
        string GetEnumTranslationKey(Enum item, string alias = null);

        /// <summary>
        /// Returns all keys defined by this provider
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetAllKeys();
    }
}