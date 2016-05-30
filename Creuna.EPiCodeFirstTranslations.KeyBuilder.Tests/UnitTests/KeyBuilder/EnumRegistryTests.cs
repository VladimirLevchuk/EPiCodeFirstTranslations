using System;
using FluentAssertions;
using NUnit.Framework;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Tests.KeyBuilder
{
    public class EnumRegistryTests
    {
        public IEnumRegistry EnumRegistry { get; set; }

        [SetUp]
        public void Setup()
        {
            EnumRegistry = new EnumRegistry();
        }

        [Test]
        public void RegisteringTwoEnums_ThrowsAnException()
        {
            EnumRegistry.Add<MyEnum>();

            Action registeringSecondEnumWithSameName = () => EnumRegistry.Add<Nested.MyEnum>();

            registeringSecondEnumWithSameName.ShouldThrow<Exception>();
        }
    }
}