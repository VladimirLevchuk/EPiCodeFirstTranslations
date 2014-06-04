using Creuna.EPiCodeFirstTranslations;
using EPiServer.ServiceLocation;

namespace $rootnamespace$.Translation
{
    [ServiceConfiguration(typeof(ITranslationService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class TranslationService : TranslationServiceBase<Translations>
    {
    }
}