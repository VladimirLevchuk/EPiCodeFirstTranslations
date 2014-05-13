using EPiServer.ServiceLocation;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Translation
{
    [ServiceConfiguration(typeof(ITranslationService))]
    public class TranslationService : TranslationServiceBase<Translations>
    {
    }
}