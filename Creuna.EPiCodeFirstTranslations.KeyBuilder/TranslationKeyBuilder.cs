using System;
using System.Linq.Expressions;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    public class TranslationKeyBuilder<TTranslationContent> : ITranslationKeyBuilder<TTranslationContent>
    {
        private readonly ITranslationsKeyMapper _keyMapper;

        public TranslationKeyBuilder(ITranslationsKeyMapper keyMapper)
        {
            _keyMapper = keyMapper;
        }

        public virtual string GetTranslationKey(Expression<Func<TTranslationContent, string>> translationPath)
        {
            var propertyPath = ExpressionUtils.GetPropertyPath(translationPath);
            var translationKey = _keyMapper.GetTranslationKey(typeof(TTranslationContent), propertyPath);
            return translationKey;
        }

        public virtual string GetEnumTranslationKey(Enum item, string alias = null)
        {
            var enumType = item.GetType();
            alias = alias ?? enumType.Name;
            var enumKey = _keyMapper.GetTranslationKey(enumType, item.ToString(), alias);
            return enumKey;
        }
    }
}
