using System.Globalization;

namespace Creuna.EPiCodeFirstTranslations
{
    public interface ITranslationContent
    {
        CultureInfo Culture { get; }
    }
}