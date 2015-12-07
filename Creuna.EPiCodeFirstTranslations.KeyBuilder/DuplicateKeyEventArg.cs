using System;
using System.Collections.Generic;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    public class KeyMapperEventArg : EventArgs
    {
        public Dictionary<string, string> TranslationKeyToPropertyPathMap { get; set; }
        public Dictionary<string, string> PropertyPathToTranslationKeyMap { get; set; }
    }

    public class DuplicateKeyEventArg : KeyMapperEventArg
    {
        public string TranslationKey { get; set; }
        public string PropertyKey { get; set; }
    }
}