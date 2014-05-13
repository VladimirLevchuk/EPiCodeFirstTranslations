using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Creuna.EPiCodeFirstTranslations.Presentation.Models.Pages;
using Creuna.EPiCodeFirstTranslations.Presentation.Translation;
using Creuna.EPiCodeFirstTranslations.Presentation.ViewModels.Pages;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Controllers
{
    public class FrontController : PageController<FrontPage>
    {
        protected virtual TranslationService Translations { get; private set; }

        public FrontController()
        {
            Translations = ServiceLocator.Current.GetInstance<TranslationService>();
        }

        public ActionResult Index(FrontPage currentPage)
        {
            return View(new FrontPageViewModel() { Header = Translations.Translate(t => t.Texts.Hello) });
        }
    }
}