using System.Globalization;
using Creuna.EPiCodeFirstTranslations.Attributes;
using EPiServer.Framework.Blobs;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Translation
{
    [TranslationPath("")]
    public class Translations : ITranslationContent
    {
        public Translations()
        {
            Texts = new Texts();
            Labels = new Labels();
            Formats = new Formats();
        }

        public CultureInfo ContentCulture { get { return CultureInfo.GetCultureInfo("en"); } }

        public Texts Texts { get; private set; }

        public Labels Labels { get; private set; }

        public Formats Formats { get; private set; }
    }
}