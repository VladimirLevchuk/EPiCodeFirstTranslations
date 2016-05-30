using System;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using NUnit.Framework;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Tests.AcceptanceTests
{
    public class TranslationAnnotationMetadataProvider_Tests
    {
        public ITranslationMetadataProvider Provider { get; set; }

        [SetUp]
        public void Setup()
        {
            this.Provider = new TranslationAnnotationMetadataProvider();
        }

        [Test]
        public void GetAllKeys_WorksForSimpleType()
        {
            Provider.GetAllKeys(typeof(Errors)).Should().BeEquivalentTo(
                "/Errors/Error1"
                , "/Errors/Error2"
                , "/Errors/Error3");
        }

        [Test]
        public void GetAllKeys_SupportsAlsoTranslationForKey()
        {
            Provider.GetAllKeys(typeof(Texts)).Should().BeEquivalentTo(
                "/my-texts/Text1"
                , "/my-texts/Text2"
                , "/custom-key/text-2");
        }

        [Test]
        public void GetAllKeys_WorksForNestedTypes()
        {
            Provider.GetAllKeys(typeof(Labels)).Should().BeEquivalentTo(
                "/Labels/Label1"
                , "/Labels/NestedLabels/NestedLabel1");
        }


        public class RecursiveTranslations
        {
            public string Text => "Test";

            public RecursiveTranslations SelfReference { get; set; }
        }

        [Test]
        public void GetAllKeys_WithRecursiveType_ThrowsException()
        {
            Action getAllKeys = () => Provider.GetAllKeys(typeof (RecursiveTranslations));
            getAllKeys.ShouldThrow<Exception>();
        }
    }
}