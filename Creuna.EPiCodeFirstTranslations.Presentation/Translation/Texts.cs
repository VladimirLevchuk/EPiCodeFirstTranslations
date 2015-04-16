using Creuna.EPiCodeFirstTranslations.Attributes;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Translation
{
    [TranslationPath("General")]
    public class Texts
    {
        public string Hello { get { return "Hello everybody!"; } }

        [TranslationForCulture("sv", "Bye for SV from CF")]
        [TranslationKey("Goodbye")] // additional key.
        public string Bye { get { return "Bye"; } }
    }
}