using System;
using System.Globalization;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Annotation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class TranslationForCultureAttribute : Attribute
    {
        public TranslationForCultureAttribute(string cultureName, string translation)
        {
            Culture = CultureInfo.GetCultureInfo(cultureName);
            Translation = translation;
        }

        public CultureInfo Culture { get; private set; }

        public string Translation { get; private set; }
    }
}