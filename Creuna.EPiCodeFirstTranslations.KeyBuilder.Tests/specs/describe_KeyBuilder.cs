using FluentAssertions;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Tests
{
    //// TODO: [high]
    ////convert to classical UT
    //public class describe_KeyBuilder : KeyBuilder_spec
    //{
    //    private ITranslationsKeyMapper _mapper;

    //    protected virtual void before_all()
    //    {
    //        this._mapper = new TranslationsKeyMapper();
    //    }

    //    protected virtual ITranslationKeyBuilder<TTranslationContent> CreateBuilderUnderTest<TTranslationContent>()
    //    {
    //        return new TranslationKeyBuilder<TTranslationContent>(_mapper);
    //    }

    //    protected void when_asking_keys_for_the_root_translations_class()
    //    {
    //        ITranslationKeyBuilder<Translations> translationsKeyBuilder = null;

    //        before = () => translationsKeyBuilder = CreateBuilderUnderTest<Translations>();

    //        it["it uses a property name to a key"] = () => translationsKeyBuilder.GetTranslationKey(x => x.Text1).Should().Be("/Text1");
    //        it["it builds a path to nested property"] = () => translationsKeyBuilder.GetTranslationKey(x => x.Labels.Label1).Should().Be("/Labels/Label1");
    //        it["it allows to override path at some point using TranslationPath attribute"] = () => translationsKeyBuilder.GetTranslationKey(x => x.Errors.Error1).Should().Be("/my-errors/Error1");
    //        it["it allows to override path using absolute path (prefixed with ~) with TranslationPath attribute"] = () => translationsKeyBuilder.GetTranslationKey(x => x.Texts.Text1).Should().Be("/my-texts/Text1");
    //        /* it's a limitation for now */
    //        it["it allows to have several properties of the same type"] = () => translationsKeyBuilder.GetTranslationKey(x => x.MoreLabels.Label1).Should().Be("/MoreLabels/Label1");
    //        it["it allows to have properties of the inherited type"] = () => translationsKeyBuilder.GetTranslationKey(x => x.AlternateLabels.Label1).Should().Be("/AlternateLabels/Label1");
    //    }

    //    protected void when_inherited_attributes_used()
    //    {
    //        ITranslationKeyBuilder<Translations> translationsKeyBuilder = null;
    //        ITranslationKeyBuilder<Messages> messagesKeyBuilder = null;

    //        before = () =>
    //        {
    //            translationsKeyBuilder = CreateBuilderUnderTest<Translations>();
    //            messagesKeyBuilder = CreateBuilderUnderTest<Messages>();
    //        };

    //        it["InheritedTranslationPath works as TranslationPath"] = () =>
    //        {
    //            translationsKeyBuilder.GetTranslationKey(x => x.Messages.Message1).Should().Be("/my-messages/Message1");
    //            messagesKeyBuilder.GetTranslationKey(x => x.Message1).Should().Be("/my-messages/Message1");
    //        };
    //    }

    //    protected void when_asking_for_keys_for_a_nested_translation_class()
    //    {
    //        ITranslationKeyBuilder<Labels> labelsKeyBuilder = null;
    //        ITranslationKeyBuilder<NestedLabels> nestedLabelsKeyBuilder = null;
    //        ITranslationKeyBuilder<Errors> errorsKeyBuilder = null;

    //        before = () =>
    //        {
    //            labelsKeyBuilder = CreateBuilderUnderTest<Labels>();
    //            nestedLabelsKeyBuilder = CreateBuilderUnderTest<NestedLabels>();
    //            errorsKeyBuilder = CreateBuilderUnderTest<Errors>();
    //        };

    //        it["it combines a class name and a property name to a key by default"] = () => labelsKeyBuilder.GetTranslationKey(x => x.Label1).Should().Be("/Labels/Label1");
    //        it["it allows to override path using TranslationPath attribute at some point"] = () => errorsKeyBuilder.GetTranslationKey(x => x.Error1).Should().Be("/my-errors/Error1");
    //        it["it allows me to share a nested translation class"] = () => nestedLabelsKeyBuilder.GetTranslationKey(x => x.NestedLabel1).Should().Be("/Labels/NestedLabels/NestedLabel1");
    //    }

    //    protected void when_asking_for_enum_keys()
    //    {
    //        ITranslationKeyBuilder<MyEnum> enumKeyBuilder = null;

    //        before = () =>
    //        {
    //            enumKeyBuilder = CreateBuilderUnderTest<MyEnum>();
    //        };

    //        it["it works"] = () => enumKeyBuilder.GetEnumTranslationKey(MyEnum.EnumValue1).Should().Be("/Enums/MyEnum/EnumValue1");
    //        it["it allows to use alias"] = () => enumKeyBuilder.GetEnumTranslationKey(MyEnumWithAlias.EnumValue1, "MyEnumAlias").Should().Be("/Enums/MyEnumAlias/EnumValue1");
    //    }
    //}
}
