using Creuna.EPiCodeFirstTranslations.Attributes;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Tests
{
    [TranslationPath("")]
    public class Translations
    {
        public Translations()
        {
            Errors = new Errors();
            Labels = new Labels();
            Texts = new Texts();
        }

        public Texts Texts { get; private set; }

        public Labels Labels { get; private set; }
        public Errors Errors { get; private set; }
        
        public string Text1 { get { return "Text1"; } }
        public string Text2 { get { return "Text2"; } }
    }

    public enum MyEnum
    {
        EnumValue1,
        EnumValue2
    }

    public enum MyEnumWithAlias
    {
        EnumValue1,
        EnumValue2
    }

    public class Labels
    {
        public Labels()
        {
            NestedLabels = new NestedLabels();
        }

        public string Label1 { get { return "Label1"; } }
        public NestedLabels NestedLabels { get; private set; }
    }

    [TranslationPath("~/Labels/NestedLabels")]
    public class NestedLabels
    {
        public string NestedLabel1 { get { return "NestedLabel1"; } }
    }

    [TranslationPath("~/my-texts")]
    public class Texts
    {
        public string Text1 { get; set; }

        [TranslationKey("/custom-key/text-2")]
        public string Text2 { get; set; }
    }

    [TranslationPath("my-errors")]
    public class Errors
    {
        public string Error1 { get { return "Error1"; } }

        public string Error2 { get { return "Error2"; } }

        [TranslationKey("/custom-key/error-3")]
        public string Error3 { get { return "Error3"; } }
    }
}