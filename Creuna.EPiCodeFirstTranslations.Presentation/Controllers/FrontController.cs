using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Creuna.EPiCodeFirstTranslations.Presentation.Models.Pages;
using Creuna.EPiCodeFirstTranslations.Presentation.Orchestrators;
using Creuna.EPiCodeFirstTranslations.Presentation.Translation;
using Creuna.EPiCodeFirstTranslations.Presentation.ViewModels.Pages;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Controllers
{
    public class FrontController : PageController<FrontPage>
    {
        protected virtual FrontOrchestrator Orchestrator { get; private set; }

        public FrontController()
        {
            Orchestrator = ServiceLocator.Current.GetInstance<FrontOrchestrator>();
        }

        public ActionResult Index(FrontPage currentPage)
        {
            return View(Orchestrator.GetPageModel());
        }
    }
}