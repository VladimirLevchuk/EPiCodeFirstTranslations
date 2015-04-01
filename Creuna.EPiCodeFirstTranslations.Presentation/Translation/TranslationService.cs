using System;
using System.Collections.Generic;
using Creuna.EPiCodeFirstTranslations.Presentation.Enums;
using EPiServer.ServiceLocation;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Translation
{
    [ServiceConfiguration(typeof(ITranslationService), Lifecycle = ServiceInstanceScope.Singleton)]
    public class TranslationService : TranslationServiceBase<Translations>
    {
        public override IEnumerable<Type> GetTranslatableEnumTypes()
        {
            return new Type[]
            {
                typeof (Gender),
                typeof(Position)
            };
        }
    }
}