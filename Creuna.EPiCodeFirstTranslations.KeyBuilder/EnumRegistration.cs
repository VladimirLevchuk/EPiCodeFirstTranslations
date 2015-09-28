using System;

namespace Creuna.EPiCodeFirstTranslations
{
    public class EnumRegistration
    {
        public EnumRegistration(Type enumType, string @alias = null)
        {
            EnumType = enumType;
            Alias = alias ?? EnumType.Name;
        }

        public Type EnumType { get; private set; }

        public string Alias { get; private set; }
    }
}