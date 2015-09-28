﻿using Creuna.EPiCodeFirstTranslations.Attributes;
using FluentAssertions;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Tests
{
    class describe_KeyBuilder : KeyBuilder_spec
    {
        private ITranslationsKeyMapper _mapper;

        void before_all()
        {
            this._mapper = new TranslationsKeyMapper();
        }

        void when_asking_keys_for_the_root_translations_class()
        {
            TranslationKeyBuilder<Translations> translationsKeyBuilder = null;

            before = () => translationsKeyBuilder = new TranslationKeyBuilder<Translations>(_mapper);

            it["it uses a property name to a key"] = () => translationsKeyBuilder.GetTranslationKey(x => x.Text1).Should().Be("/Text1");
            it["it builds a path to nested property"] = () => translationsKeyBuilder.GetTranslationKey(x => x.Labels.Label1).Should().Be("/Labels/Label1");
            it["it allows to override path at some point using TranslationPath attribute"] = () => translationsKeyBuilder.GetTranslationKey(x => x.Errors.Error1).Should().Be("/my-errors/Error1");
            it["it allows to override path using absolute path (prefixed with ~) with TranslationPath attribute"] = () => translationsKeyBuilder.GetTranslationKey(x => x.Texts.Text1).Should().Be("/my-texts/Text1");
        }

        void when_asking_for_keys_for_a_nested_translation_class()
        {
            TranslationKeyBuilder<Labels> labelsKeyBuilder = null;
            TranslationKeyBuilder<NestedLabels> nestedLabelsKeyBuilder = null;
            TranslationKeyBuilder<Errors> errorsKeyBuilder = null;

            before = () =>
            {
                labelsKeyBuilder = new TranslationKeyBuilder<Labels>(_mapper);
                nestedLabelsKeyBuilder = new TranslationKeyBuilder<NestedLabels>(_mapper);
                errorsKeyBuilder = new TranslationKeyBuilder<Errors>(_mapper);
            };

            it["it combines a class name and a property name to a key by default"] = () => labelsKeyBuilder.GetTranslationKey(x => x.Label1).Should().Be("/Labels/Label1");
            it["it allows to override path using TranslationPath attribute at some point"] = () => errorsKeyBuilder.GetTranslationKey(x => x.Error1).Should().Be("/my-errors/Error1");
            it["it allows me to share a nested translation class"] = () => nestedLabelsKeyBuilder.GetTranslationKey(x => x.NestedLabel1).Should().Be("/Labels/NestedLabels/NestedLabel1");
        }

        void when_asking_for_enum_keys()
        {
            TranslationKeyBuilder<MyEnum> enumKeyBuilder = null;

            before = () =>
            {
                enumKeyBuilder = new TranslationKeyBuilder<MyEnum>(_mapper);
            };

            it["it works"] = () => enumKeyBuilder.GetEnumTranslationKey(MyEnum.EnumValue1).Should().Be("/Enums/MyEnum/EnumValue1") ;
            it["it allows to use alias"] = () => enumKeyBuilder.GetEnumTranslationKey(MyEnumWithAlias.EnumValue1, "MyEnumAlias").Should().Be("/Enums/MyEnumAlias/EnumValue1");
        }
    }
}