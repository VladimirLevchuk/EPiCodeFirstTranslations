using Creuna.EPiCodeFirstTranslations.Attributes;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Translation
{
    [KeyBuilder.Annotation.TranslationPath("General")]
    public class Texts
    {
        public string Hello { get { return "Hello everybody!"; } }

#pragma warning disable 618
        [TranslationForCulture("sv", "Bye for SV from CF")]
#pragma warning restore 618
        [KeyBuilder.Annotation.AlsoTranslationForKey("Goodbye")] // additional key.
        public string Bye { get { return "Bye"; } }
    }
}