using System.Collections.Specialized;
using System.Linq;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;

namespace Creuna.EPiCodeFirstTranslations
{
    [InitializableModule]
    [ModuleDependency(typeof(FrameworkInitialization))]
    public class CodeFirstLocalizationProviderInitialization  : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            // Casts the current LocalizationService to a ProviderBasedLocalizationService to get access to the current list of providers.
            var localizationService = ServiceLocator.Current.GetInstance<LocalizationService>() as ProviderBasedLocalizationService;
            if (localizationService != null)
            {
                var codeFirstLocalizationProvider = localizationService.Providers.FirstOrDefault(p => p is CodeFirstLocalizationProvider);
                if (codeFirstLocalizationProvider == null)
                {
                    codeFirstLocalizationProvider = context.Locate.Advanced.GetInstance<CodeFirstLocalizationProvider>();
                    codeFirstLocalizationProvider.Initialize(typeof(CodeFirstLocalizationProvider).Name, new NameValueCollection());
                    localizationService.Providers.Add(codeFirstLocalizationProvider);
                }
            }
        }

        public void Uninitialize(InitializationEngine context)
        {
           
        }

        public void Preload(string[] parameters)
        {
           
        }
    }
}
