using Creuna.EPiCodeFirstTranslations;
using EPiServer.ServiceLocation;

namespace $rootnamespace$.Translation
{
    [ServiceConfiguration(typeof(ITranslationService))]
    public class TranslationService : TranslationServiceBase<Translations>
    {
    }
}