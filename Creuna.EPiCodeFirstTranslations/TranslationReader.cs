using System;
using System.Globalization;

namespace Creuna.EPiCodeFirstTranslations
{
    public class TranslationReader
    {
        public string GetTranslation(ITranslationContent translationContent, string translationValueKey, CultureInfo culture)
        {
            if (translationContent == null)
            {
                throw new ArgumentNullException("translationContent");
            }
            
            if (string.IsNullOrEmpty(translationValueKey))
            {
                throw new ArgumentNullException("translationValueKey");
            }

            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }

            if (!culture.Equals(translationContent.Culture))
            {
                return null;
            }

            object propertyValue = GetPropertyValue(translationContent, translationValueKey);
            if (propertyValue != null)
            {
                return propertyValue.ToString();
            }

            return null;
        }

        protected virtual object GetPropertyValue(object instance, string propertyPath)
        {
            string[] propertyNames = propertyPath.Split('.');

            foreach (var propertyName in propertyNames)
            {
                if (instance == null)
                {
                    return null;
                }

                var property = instance.GetType().GetProperty(propertyName);
                instance = property.GetValue(instance, null);
            }

            return instance;
        }
    }
}