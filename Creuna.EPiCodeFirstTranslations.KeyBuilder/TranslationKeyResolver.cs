using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using JetBrains.Annotations;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    public class TranslationKeyResolver<TTranslationContent> : ITranslationKeyBuilder<TTranslationContent>
        where TTranslationContent: new()
    {
        private readonly ITranslationMetadataProvider _translationMetadataProvider;
        private readonly IEnumTranslationMetadataProvider _enumTranslationMetadataProvider;

        public TranslationKeyResolver(ITranslationMetadataProvider translationMetadataProvider,
            IEnumTranslationMetadataProvider enumTranslationMetadataProvider)
        {
            _translationMetadataProvider = translationMetadataProvider;
            _enumTranslationMetadataProvider = enumTranslationMetadataProvider;
        }


        public virtual string GetTranslationKey([NotNull] Expression<Func<TTranslationContent, string>> translationPath)
        {
            var propertyPath = ExpressionUtils.GetPropertyPath(translationPath);
            return GetTranslationKey(propertyPath);
        }

        public virtual string GetTranslationKey(IEnumerable<string> propertyPath)
        {
            var currentPath = new List<string>();
            var translationsType = typeof(TTranslationContent);
            var currentKey = _translationMetadataProvider.GetTranslationsRootPath(translationsType);

            foreach (var currentProperty in propertyPath)
            {
                currentPath.Add(currentProperty);
                currentKey = TranslationPath.Combine(currentKey,
                    _translationMetadataProvider.GetTranslationKey(ExpressionUtils.GetPropertyInfo(typeof(TTranslationContent), currentPath)));
            }

            return currentKey;
        }

        public virtual string GetEnumTranslationKey(Enum item, [CanBeNull] string alias = null)
        {
            return _enumTranslationMetadataProvider.GetEnumTranslationKey(item, alias);
        }

        public virtual IEnumerable<string> GetAllKeys()
        {
            return _translationMetadataProvider.GetAllKeys(typeof(TTranslationContent)).Union(_enumTranslationMetadataProvider.GetAllKeys());
        }
    }
}