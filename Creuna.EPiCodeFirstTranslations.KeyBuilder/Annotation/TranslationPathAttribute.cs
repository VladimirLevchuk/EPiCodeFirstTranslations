using System;

namespace Creuna.EPiCodeFirstTranslations.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class TranslationPathAttribute : Attribute
    {
        public TranslationPathAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }
}