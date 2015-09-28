using System;

namespace Creuna.EPiCodeFirstTranslations.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class TranslationKeyAttribute : Attribute
    {
        public TranslationKeyAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; private set; }
    }
}