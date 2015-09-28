using Creuna.EPiCodeFirstTranslations.KeyBuilder;
using StructureMap.Configuration.DSL;

namespace Creuna.EPiCodeFirstTranslations
{
    public class EPiCodeFirstTranslationsRegistry : Registry
    {
        public EPiCodeFirstTranslationsRegistry()
        {
            For<ITranslationsKeyMapper>().Singleton().Use<TranslationsKeyMapper>();
            For<IEnumRegistry>().Singleton().Use<EnumRegistry>();

            For<ITranslationProvider>().Use<TranslationProvider>();
        }
    }

    public class EPiCodeFirstTranslationsRegistry<TTranslationContent> : EPiCodeFirstTranslationsRegistry
    {
        public EPiCodeFirstTranslationsRegistry()
        {
            For<ITranslationKeyBuilder<TTranslationContent>>().Use<TranslationKeyBuilder<TTranslationContent>>();
        }
    }

    public class EPiCodeFirstTranslationServiceRegistry<TTranslationService, TTranslations> : Registry
        where TTranslationService: TranslationServiceBase<TTranslations>
    {
        public EPiCodeFirstTranslationServiceRegistry()
        {
            For<TTranslationService>().Singleton().Use<TTranslationService>();
            For<ITranslationService>().Singleton().Use(ctx => ctx.GetInstance<TTranslationService>());
            For<ITranslationService<TTranslations>>().Singleton().Use(ctx => ctx.GetInstance<TTranslationService>());
        }
    }
}