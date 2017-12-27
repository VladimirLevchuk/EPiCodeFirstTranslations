Creuna.EPiCodeFirstTranslations
===============================

NOTE: starting from v4.0.0 EPiCodeFirstTranslationsRegistry is not a part of the package, so you'll need to define it yourself 
(or setup your IoC container in any other way you prefer)

Configure your IoC before using:

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

	public class EPiCodeFirstTranslationsRegistry : Registry
    {
        public EPiCodeFirstTranslationsRegistry()
        {
            For<ITranslationsKeyMapper>().Singleton().Use<TranslationsKeyMapper>();

            var translatableEnums = new EnumRegistry();

            // add your translatable enums here
            // i.e. 
			// translatableEnums.Add<Gender>();
            For<IEnumRegistry>().Singleton().Use(translatableEnums);

            For<ITranslationProvider>().Use<TranslationProvider>();
            For<ITranslationKeyBuilder<Translations>>().Use<TranslationKeyBuilder<Translations>>();

            For<TranslationService>().Singleton().Use<TranslationService>();
			// do not forget to configure interfaces
			Forward<TranslationService, ITranslationService>();
            Forward<TranslationService, ITranslationService<Translations>>();
        }
    }