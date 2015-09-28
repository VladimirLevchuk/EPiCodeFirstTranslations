using System;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Annotation
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class AlsoTranslationForKeyAttribute : Attribute
    {
        public AlsoTranslationForKeyAttribute(string key)
        {
            Key = key;
        }

        public string Key { get; private set; }
    }
}