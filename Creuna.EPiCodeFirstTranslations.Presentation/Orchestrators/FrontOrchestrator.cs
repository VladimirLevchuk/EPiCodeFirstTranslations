using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Creuna.EPiCodeFirstTranslations.Presentation.Translation;
using Creuna.EPiCodeFirstTranslations.Presentation.ViewModels.Pages;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Orchestrators
{
    public class FrontOrchestrator
    {
        public FrontOrchestrator(ITranslationService<Translations> translations)
        {
            Translations = translations;
        }

        protected virtual ITranslationService<Translations> Translations { get; private set; }

        public FrontPageViewModel GetPageModel()
        {
            return new FrontPageViewModel { Header = Translations.Translate(t => t.Labels.HelloWorld) };
        }
    }
}