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
        [TranslationForCulture("sv", "None for SV from CF")]
        [Display(Name = "None Display Name")]
        None = 0,

        [Display(Name = "Winter")]
        Winter = 1 << 0,

        [Display(Name = "Spring default translation")]
        Spring = 1 << 1,

        [Display(Name = "Summer")]
        Summer = 1 << 2,

        [Display(Name = "Autumn")]
        Autumn = 1 << 3,

        [Display(Name = "All year")]
        AllYear = Winter | Spring | Summer | Autumn
    }
}