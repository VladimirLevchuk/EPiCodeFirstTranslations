using FluentAssertions;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Tests
{
    class describe_TranslationkeyMapper : KeyBuilder_spec
    {
        private ITranslationsKeyMapper _mapper;

        void before_all()
        {
            this._mapper = new TranslationsKeyMapper();
        }

        void when_mapper_created()
        {
            it["it allows to query all keys for translation class"] = () =>
            {
                var map = _mapper.QueryTranslationKeyToPropertyPathMap(typeof (Errors), string.Empty);
                map.Keys.Should().BeEquivalentTo("/my-errors/Error1", "/my-errors/Error2", "/my-errors/Error3", "/my-errors/custom-key/error-3");
            };

            it["it allows to get all keys started from the given translation path for translation class"] = () =>
            {
                var map = _mapper.QueryTranslationKeyToPropertyPathMap(typeof(Translations), "/my-errors");
                map.Keys.Should().BeEquivalentTo("/my-errors/Error1", "/my-errors/Error2", "/my-errors/Error3", "/my-errors/custom-key/error-3");
            };

            it["it works with enum"] = () =>
            {
                var map = _mapper.QueryTranslationKeyToPropertyPathMap(typeof(MyEnum), string.Empty);
                map.Keys.Should().BeEquivalentTo("/Enums/MyEnum/EnumValue1", "/Enums/MyEnum/EnumValue2");
            };

            it["it works with enum with alias"] = () =>
            {
                var map = _mapper.QueryTranslationKeyToPropertyPathMap(typeof(MyEnumWithAlias), string.Empty, "MyEnumAlias");
                map.Keys.Should().BeEquivalentTo("/Enums/MyEnumAlias/EnumValue1", "/Enums/MyEnumAlias/EnumValue2");
            };

            it["InheritedAlsoTranslationForKeyAttribute works as AlsoTranslationForKeyAttribute"] = () =>
            {
                var map = _mapper.QueryTranslationKeyToPropertyPathMap(typeof(Translations), "/my-messages");
                map.Keys.Should().BeEquivalentTo("/my-messages/Message1", "/my-messages/Message2", "/my-messages/inherited-key/text-2");
            };
        }
    }
}