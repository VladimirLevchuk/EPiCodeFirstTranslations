using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace Creuna.EPiCodeFirstTranslations.Presentation
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
            context.StructureMap().Configure(x => x.AddRegistry<EPiCodeFirstTranslationsRegistry>());
        }
    }
}