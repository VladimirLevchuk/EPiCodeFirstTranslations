using System.Collections.Generic;
using System.Linq;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    public static class OrEmptyExtensions
    {
        public static T OrEmpty<T>(this T @object)
            where T: class, new()
        {
            return @object.Or(new T());
        }

        public static T Or<T>(this T @object, T fallback)
            where T : class
        {
            return @object ?? fallback;
        }

        public static IEnumerable<T> OrEmpty<T>(IEnumerable<T> enumerable)
        {
            return enumerable ?? Enumerable.Empty<T>();
        }
    }
}