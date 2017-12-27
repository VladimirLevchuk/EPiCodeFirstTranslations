using Creuna.EPiCodeFirstTranslations.KeyBuilder;
using Creuna.EPiCodeFirstTranslations.Presentation.Enums;
using Creuna.EPiCodeFirstTranslations.Presentation.Translation;
using StructureMap;

namespace Creuna.EPiCodeFirstTranslations.Presentation
{
    public class EPiCodeFirstTranslationsRegistry : Registry
    {
        public EPiCodeFirstTranslationsRegistry()
        {
            For<ITranslationsKeyMapper>().Singleton().Use<TranslationsKeyMapper>();

            var translatableEnums = new EnumRegistry();

            // add your translatable enums here
            translatableEnums.Add<Gender>();
            translatableEnums.Add<Enums2.Gender>("Gender 2");
            translatableEnums.Add<Position>();
            translatableEnums.Add<Season>();
            For<IEnumRegistry>().Singleton().Use(translatableEnums);

            For<ITranslationProvider>().Use<TranslationProvider>();
            For<ITranslationKeyBuilder<Translations>>().Use<TranslationKeyBuilder<Translations>>();

            For<TranslationService>().Singleton().Use<TranslationService>();
            Forward<TranslationService, ITranslationService>();
            Forward<TranslationService, ITranslationService<Translations>>();
        }
    }
}