using System;
using System.Collections.Generic;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    public class TranslationKeyMapperDump
    {
        public Dictionary<Type, Dictionary<string, string>> PropertyPathToTranslationKeyMaps { get; set; }
        public Dictionary<Type, Dictionary<string, string>> TranslationKeyToPropertyPathMaps { get; set; }
    }
}