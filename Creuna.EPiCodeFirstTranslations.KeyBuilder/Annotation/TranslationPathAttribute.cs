using System;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Annotation
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class TranslationPathAttribute : Attribute
    {
        public TranslationPathAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }

        public string AbsolutePath
        {
            get { return Path.Replace("~", ""); }
        }

        public bool IsAbsolute
        {
            get { return Path != null && Path.StartsWith("~"); }
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class TranslationPropertyPathAttribute : TranslationPathAttribute
    {
        public TranslationPropertyPathAttribute(string path) : base(path)
        {
        }
    }

    //[AttributeUsage(AttributeTargets.Enum, Inherited = false, AllowMultiple = true)]
    //public class TranslationEnumAttribute : TranslationPathAttribute
    //{
    //    public TranslationEnumAttribute(string path = null) : base(path)
    //    {
    //    }
    //}

    //[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    //public class TranslationEnumItemPathAttribute : TranslationPathAttribute
    //{
    //    public TranslationEnumItemPathAttribute(string path) : base(path)
    //    {
    //    }
    //}
}