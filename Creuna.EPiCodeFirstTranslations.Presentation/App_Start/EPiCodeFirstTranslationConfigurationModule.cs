using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Creuna.EPiCodeFirstTranslations.KeyBuilder;
using Creuna.EPiCodeFirstTranslations.Presentation.Enums;
using Creuna.EPiCodeFirstTranslations.Presentation.Translation;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using StructureMap.Configuration.DSL;

namespace Creuna.EPiCodeFirstTranslations.Presentation.App_Start
{
    [InitializableModule]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class EPiCodeFirstTranslationConfigurationModule : IConfigurableModule
    {
        public virtual void Initialize(InitializationEngine context)
        {}

        public virtual void Uninitialize(InitializationEngine context)
        {}

        public virtual void Preload(string[] parameters)
        {}

        public virtual void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(x => x.AddRegistry(new EPiCodeFirstTranslationsRegistry<Translations>()));
            context.Container.Configure(x => x.AddRegistry(new WebAppRegistry()));
        }
    }

    public class WebAppRegistry : Registry
    {
        public WebAppRegistry()
        {
            var translatableEnums = new EnumRegistry();

            translatableEnums.Add<Gender>();
            translatableEnums.Add<Enums2.Gender>("Gender 2");
            translatableEnums.Add<Position>();
            translatableEnums.Add<Season>();

            For<IEnumRegistry>().Singleton().Use(translatableEnums);

            For<TranslationService>().Singleton().Use<TranslationService>();
            For<ITranslationService>().Singleton().Use(ctx => ctx.GetInstance<TranslationService>());
        }
    }
}