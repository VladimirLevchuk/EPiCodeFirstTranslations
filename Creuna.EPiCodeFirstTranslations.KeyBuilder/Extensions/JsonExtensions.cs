using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJsonString(this object value, bool ignoreNullValues = false)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var settings = GetSerializerSettings(ignoreNullValues);

            var result = JsonConvert.SerializeObject(value, settings);

            return result;
        }

        public static T FromJson<T>(this string json)
        {
            var settings = GetSerializerSettings();

            var result = JsonConvert.DeserializeObject<T>(json, settings);

            return result;
        }

        public static T TryFromJson<T>(this string json)
            where T : class
        {
            if (json == null)
            {
                return null;
            }

            var settings = GetSerializerSettings();

            try
            {
                var result = JsonConvert.DeserializeObject<T>(json, settings);
                return result;
            }
            catch (Exception /*ex*/)
            {
                return null;
            }
        }

        private static JsonSerializerSettings GetSerializerSettings(bool ignoreNullValues = false)
        {
            var result = new JsonSerializerSettings
            {
#if DEBUG
                Formatting = Formatting.Indented,
#else
                Formatting = Formatting.None,
#endif
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            if (ignoreNullValues)
            {
                result.NullValueHandling = NullValueHandling.Ignore;
            }

            return result;
        }
    }
}