using Creuna.EPiCodeFirstTranslations.KeyBuilder.Annotation;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Translation
{
    [TranslationPath("")]
    public class Translations
    {
        public Translations()
        {
            Texts = new Texts();
            Labels = new Labels();
            Formats = new Formats();
        }

        public Texts Texts { get; private set; }

        public Labels Labels { get; private set; }

        public Formats Formats { get; private set; }
    }
}