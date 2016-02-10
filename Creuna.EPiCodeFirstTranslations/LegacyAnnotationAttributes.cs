using System;
using System.Globalization;

namespace Creuna.EPiCodeFirstTranslations.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    [Obsolete("Use Creuna.EPiCodeFirstTranslations.KeyBuilder.Annotation.TranslationPathAttribute instead")]
    public sealed class TranslationPathAttribute : KeyBuilder.Annotation.TranslationPathAttribute
    {
        public TranslationPathAttribute(string path) : base(path)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    [Obsolete("Use Creuna.EPiCodeFirstTranslations.KeyBuilder.Annotation.AlsoTranslationForKeyAttribute instead")]
    public sealed class TranslationKeyAttribute : KeyBuilder.Annotation.AlsoTranslationForKeyAttribute
    {
        public TranslationKeyAttribute(string key) : base(key)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    [Obsolete("Use Creuna.EPiCodeFirstTranslations.KeyBuilder.Annotation.AlsoTranslationForKeyAttribute instead")]
    public sealed class TranslationForCultureAttribute : KeyBuilder.Annotation.TranslationForCultureAttribute
    {
        public TranslationForCultureAttribute(string cultureName, string translation)
            : base(cultureName, translation)
        {}
    }
}