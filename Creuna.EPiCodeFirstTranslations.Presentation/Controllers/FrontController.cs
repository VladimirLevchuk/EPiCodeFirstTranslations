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
using FluentAssertions;

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
            TestIoC();
            return View(Orchestrator.GetPageModel());
        }

        private void TestIoC()
        {
            var provider = ServiceLocator.Current.GetInstance<CodeFirstLocalizationProvider>();
            provider.Should().NotBeNull("Configure CodeFirstLocalizationProvider");
            var concreteService = ServiceLocator.Current.GetInstance<TranslationService>();
            concreteService.Should().NotBeNull("Configure TranslationService");
            var service = ServiceLocator.Current.GetInstance<ITranslationService<Translations>>();
            service.Should().NotBeNull("Configure ITranslationService<Translations>");
            concreteService.Should().Be(service);
        }
    }
}