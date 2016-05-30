using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Tests.AcceptanceTests
{
    public class EnumRegistryTranslationMetadataProvider_Tests
    {
        public IEnumTranslationMetadataProvider Provider { get; set; }
        public IEnumRegistry EnumRegistry { get; set; }

        [SetUp]
        public void Setup()
        {
            EnumRegistry = new EnumRegistry();
            Provider = new EnumRegistryTranslationMetadataProvider(EnumRegistry);

            EnumRegistry.Add<MyEnum>();
            EnumRegistry.Add<MyEnumWithAlias>("MyEnum2");
            EnumRegistry.Add<Nested.MyEnum>("Nested/MyEnum");
        }

        [Test]
        public void GetAllKeys_ReturnsAllRegisteredEnumKeys()
        {
            Provider.GetAllKeys().Should().BeEquivalentTo(
                "/Enums/MyEnum/EnumValue1"
                , "/Enums/MyEnum/EnumValue2"
                , "/Enums/MyEnum2/EnumValue1"
                , "/Enums/MyEnum2/EnumValue2"
                , "/Enums/Nested/MyEnum/EnumValue1"
                , "/Enums/Nested/MyEnum/EnumValue2");
        }

        [Test]
        public void GetEnumTranslationKey_WorksForEnum()
        {
            Provider.GetEnumTranslationKey(MyEnum.EnumValue1).Should().Be("/Enums/MyEnum/EnumValue1");
        }

        [Test]
        public void GetEnumTranslationKey_WorksForEnumWithAlias()
        {
            Provider.GetEnumTranslationKey(MyEnumWithAlias.EnumValue2).Should().Be("/Enums/MyEnum2/EnumValue2");
        }
    }
}
