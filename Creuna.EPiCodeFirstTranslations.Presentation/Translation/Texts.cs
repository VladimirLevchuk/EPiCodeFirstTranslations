using Creuna.EPiCodeFirstTranslations.Attributes;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Translation
{
    public class Texts
    {
        public string Hello { get { return "Hello everybody!"; } }

        [TranslationKey("Goodbye")] // additional key.
        public string Bye { get { return "Hello everybody!"; } }
    }
}