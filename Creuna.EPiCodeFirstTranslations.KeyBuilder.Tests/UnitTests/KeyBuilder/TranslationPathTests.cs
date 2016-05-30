using FluentAssertions;
using NUnit.Framework;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Tests.KeyBuilder
{
    public class TranslationPathTests
    {
        [Test]
        public void IsAbsolute_ReturnsTrue_ForPathsWithTildePrefix()
        {
            TranslationPath.IsAbsolute("~/absolute").Should().BeTrue();
        }

        [Test]
        public void IsAbsolute_ReturnsFalse_ForPathsWithTildePrefix()
        {
            TranslationPath.IsAbsolute("/relative").Should().BeFalse();
        }

        [Test]
        public void EnsureStartsWithSlash_ReturnsOriginalPath_WhenItStartsWithSlash()
        {
            TranslationPath.EnsureStartsWithSlash("/path").Should().Be("/path");
        }

        [Test]
        public void EnsureStartsWithSlash_PrependsSlashToPath()
        {
            TranslationPath.EnsureStartsWithSlash("path").Should().Be("/path");
        }

        [Test]
        public void Combine_CombinesGivenPaths()
        {
            TranslationPath.Combine("/path1", "/path2").Should().Be("/path1/path2");
        }

        [Test]
        public void Combine_DoesNotDuplicateSlashes()
        {
            TranslationPath.Combine("/path1/", "/path2").Should().Be("/path1/path2");
        }

        [Test]
        public void Combine_PrependsSlashWhenNeeded()
        {
            TranslationPath.Combine("path1", "/path2").Should().Be("/path1/path2");
        }

        [Test]
        public void Combine_AddsSlashWhenNeeded()
        {
            TranslationPath.Combine("/path1", "path2").Should().Be("/path1/path2");
        }

        [Test]
        public void Combine_IgnoresFirstPath_WhenSecondOneIsAbsolute()
        {
            TranslationPath.Combine("/path1", "~/path2").Should().Be("/path2");
        }
    }
}
