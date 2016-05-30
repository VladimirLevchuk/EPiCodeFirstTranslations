using FluentAssertions;
using NSpec;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Tests
{
    // TODO: [high] convert to classical UT
    class describe_ExpressionUtils : KeyBuilder_spec
    {
        void when_get_property_path()
        {
            it["it returns property name as path for class property of a reference type"] = () =>
                ExpressionUtils.GetPropertyPath<Translations>(t => t.Texts)
                    .Should().Be("Texts");

            it["it returns property name as path for class properties of a simple type"] = () =>
                ExpressionUtils.GetPropertyPathString((Translations t) => t.Text1)
                    .Should().Be("Text1");

            it["it returns dot-separated names as path for nested properties"] = () =>
                ExpressionUtils.GetPropertyPathString((Translations t) => t.MoreLabels.NestedLabels.NestedLabel1)
                    .Should().Be("MoreLabels.NestedLabels.NestedLabel1"); 
        }
    }
}
