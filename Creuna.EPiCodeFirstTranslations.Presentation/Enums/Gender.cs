using System;
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

    [Flags]
    public enum Season
    {
        [Display(Name = "None")]
        None = 0,

        [Display(Name = "Winter")]
        Winter = 1,

        [Display(Name = "Spring")]
        Spring = 2,

        [Display(Name = "Summer")]
        Summer = 4,

        [Display(Name = "Autumn")]
        Autumn = 8,

        [Display(Name = "All year")]
        AllYear = Winter & Spring & Summer & Autumn
    }
}