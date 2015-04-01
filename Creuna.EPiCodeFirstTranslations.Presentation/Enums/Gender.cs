using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Creuna.EPiCodeFirstTranslations.Attributes;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Enums2
{
    public enum Gender
    {
        Male,

        Female
    }
}

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