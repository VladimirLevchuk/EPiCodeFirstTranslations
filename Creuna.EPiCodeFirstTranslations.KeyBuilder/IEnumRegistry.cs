using System;
using System.Collections.Generic;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    public interface IEnumRegistry
    {
        void RegisterEnumAsTranslatable(EnumRegistration registration);
        void AddTranslatableEnum(Type enumType, string alias = null);
        void Add<TEnum>(string alias = null);
        IEnumerable<EnumRegistration> GetTranslatableEnumTypeRegistrations();
        EnumRegistration TryGetEnumRegistration(Type enumType);
    }
}