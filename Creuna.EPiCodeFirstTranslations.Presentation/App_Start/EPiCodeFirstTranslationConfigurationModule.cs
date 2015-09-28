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
            
            var translatableEnums = new EnumRegistry();

            // add your translatable enums here
            translatableEnums.Add<Gender>();
            translatableEnums.Add<Enums2.Gender>("Gender 2");
            translatableEnums.Add<Position>();
            translatableEnums.Add<Season>();

            context.Container.Configure(x => x.For<IEnumRegistry>().Singleton().Use(translatableEnums));

            context.Container.Configure(x => x.AddRegistry(new EPiCodeFirstTranslationServiceRegistry<TranslationService, Translations>()));
        }
    }
}