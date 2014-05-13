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
            Culture = CultureInfo.GetCultureInfo("en");
            Texts = new Texts();
            Labels = new Labels();
        }

        public CultureInfo Culture { get; private set; }

        public Texts Texts { get; private set; }

        public Labels Labels { get; private set; }
    }

    public class Texts
    {
        public string Hello { get { return "Hello everybody!"; } }
    }

    public class Labels
    {
        public string Ok { get { return "Ok!!!"; } }

        public string Cancel { get { return "Cancel!!!"; } }
    }
}