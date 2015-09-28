using System;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Annotation
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class TranslationPathAttribute : Attribute
    {
        public TranslationPathAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }
}