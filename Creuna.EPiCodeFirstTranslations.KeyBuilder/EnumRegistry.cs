using System;
using System.Collections.Generic;
using System.Linq;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    public class EnumRegistry : IEnumRegistry
    {
        private readonly Dictionary<Type, EnumRegistration> _translatableEnumRegistrations = new Dictionary<Type, EnumRegistration>();

        public virtual void RegisterEnumAsTranslatable(EnumRegistration registration)
        {
            if (registration == null) throw new ArgumentNullException("registration");
            if (!registration.EnumType.IsEnum) throw new ArgumentException("Type is not enum.", "registration");

            if (_translatableEnumRegistrations.ContainsKey(registration.EnumType))
            {
                throw new ArgumentException("This type of enum has already been registered.");
            }

            if (_translatableEnumRegistrations.Values.Any(x => x.Alias.Equals(registration.Alias, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("Enum with the same alias has already been registered.", "registration");
            }

            _translatableEnumRegistrations.Add(registration.EnumType, registration);
        }

        public virtual void AddTranslatableEnum(Type enumType, string alias = null)
        {
            RegisterEnumAsTranslatable(new EnumRegistration(enumType, alias));
        }

        public virtual void Add<TEnum>(string alias = null)
        {
            RegisterEnumAsTranslatable(new EnumRegistration(typeof(TEnum), alias));
        }

        public virtual IEnumerable<EnumRegistration> GetTranslatableEnumTypeRegistrations()
        {
            return _translatableEnumRegistrations.Values;
        }

        public virtual EnumRegistration TryGetEnumRegistration(Type enumType)
        {
            EnumRegistration enumRegistration;

            return _translatableEnumRegistrations.TryGetValue(enumType, out enumRegistration) 
                ? enumRegistration 
                : null;
        }
    }
}