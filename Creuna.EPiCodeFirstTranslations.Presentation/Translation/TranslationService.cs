using Creuna.EPiCodeFirstTranslations.Presentation.Enums;
using EPiServer.ServiceLocation;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Translation
{
    [ServiceConfiguration(typeof(ITranslationService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class TranslationService : TranslationServiceBase<Translations>
    {
        public TranslationService()
        {
            RegisterEnumsAsTranslatable(new[]
            {
                new EnumRegistration(typeof(Gender)),
                new EnumRegistration(typeof(Enums2.Gender), "Gender 2"), 
                new EnumRegistration(typeof(Position)),
            });
        }
    }
}