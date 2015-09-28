using Creuna.EPiCodeFirstTranslations.Attributes;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Translation
{
    [KeyBuilder.Annotation.TranslationPath("General")]
    public class Texts
    {
        public string Hello { get { return "Hello everybody!"; } }

        [TranslationForCulture("sv", "Bye for SV from CF")]
        [KeyBuilder.Annotation.AlsoTranslationForKey("Goodbye")] // additional key.
        public string Bye { get { return "Bye"; } }
    }
}