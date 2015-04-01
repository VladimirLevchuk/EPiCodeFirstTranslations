using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Enums
{
    public enum Gender
    {
        Male,

        Female
    }

    public enum Position
    {
        Top,

        Bottom,

        [Display(Name = "Top Left")]
        TopLeft,

        [Display(Name = "Top Right")]
        TopRight
    }
}