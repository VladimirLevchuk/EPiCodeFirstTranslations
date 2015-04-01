using System.Globalization;
using Creuna.EPiCodeFirstTranslations;
using Creuna.EPiCodeFirstTranslations.Attributes;
using EPiServer.Framework.Blobs;

namespace $rootnamespace$.Translation
{
    /*
     * See examples of child content classes and properties commented.
     */

    [TranslationPath("")]
    public class Translations
    {
        public Translations()
        {
            //Texts = new Texts();
            //Labels = new Labels();
        }

        //public Texts Texts { get; private set; }

        //public Labels Labels { get; private set; }
    }

    // Texts class example.

    /*public class Texts
    {
        public string Hello { get { return "Hello everybody!"; } }
            
        [TranslationKey("Goodbye")] // additional key.
        public string Bye  { get { return "Hello everybody!"; } }
    }*/
}