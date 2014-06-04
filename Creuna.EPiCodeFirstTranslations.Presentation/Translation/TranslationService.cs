using EPiServer.ServiceLocation;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Translation
{
    [ServiceConfiguration(typeof(ITranslationService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class TranslationService : TranslationServiceBase<Translations>
    {
    }
}