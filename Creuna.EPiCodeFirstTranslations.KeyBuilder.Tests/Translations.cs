using Creuna.EPiCodeFirstTranslations.KeyBuilder.Annotation;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Tests
{
    [TranslationPath("")]
    public class Translations
    {
        public Translations()
        {
            Messages = new Messages();
            Errors = new Errors();
            Labels = new Labels();
            Texts = new Texts();
        }

        public Texts Texts { get; private set; }

        public Labels Labels { get; private set; }
        public Errors Errors { get; private set; }
        public Messages Messages { get; private set; }
        
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
        public string Text1 { get { return "Text1"; } }

        [AlsoTranslationForKey("/custom-key/text-2")]
        public string Text2 { get { return "Text2"; }}
    }

    [TranslationPath("my-errors")]
    public class Errors
    {
        public string Error1 { get { return "Error1"; } }

        public string Error2 { get { return "Error2"; } }

        [AlsoTranslationForKey("/custom-key/error-3")]
        public string Error3 { get { return "Error3"; } }
    }

    public class InheritedTranslationPathAttribute : TranslationPathAttribute
    {
        public InheritedTranslationPathAttribute(string path) : base(path)
        {
        }
    }

    public class InheritedAlsoTranslationForKeyAttribute : AlsoTranslationForKeyAttribute
    {
        public InheritedAlsoTranslationForKeyAttribute(string key) : base(key)
        {
        }
    }

    [InheritedTranslationPath("~/my-messages")]
    public class Messages
    {
        public string Message1 { get { return "Message1"; } }

        [InheritedAlsoTranslationForKey("/inherited-key/text-2")]
        public string Message2 { get { return "Message2"; }}
    }
}