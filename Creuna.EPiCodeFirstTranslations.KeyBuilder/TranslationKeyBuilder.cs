using System;
using System.Linq.Expressions;

namespace Creuna.EPiCodeFirstTranslations.KeyBuilder
{
    public class TranslationKeyBuilder<TTranslationContent> : ITranslationKeyBuilder<TTranslationContent>
    {
        private readonly object _syncRoot = new object();
        private readonly ITranslationsKeyMapper _keyMapper;

        public TranslationKeyBuilder(ITranslationsKeyMapper keyMapper)
        {
            if (keyMapper == null) throw new ArgumentNullException("keyMapper");
            _keyMapper = keyMapper;
        }

        public virtual string GetTranslationKey(Expression<Func<TTranslationContent, string>> translationPath)
        {
            lock (_syncRoot)
            {
                var propertyPath = ExpressionUtils.GetPropertyPath(translationPath);
                var translationKey = _keyMapper.GetTranslationKey(typeof (TTranslationContent), propertyPath);
                return translationKey;
            }
        }

        public virtual string GetEnumTranslationKey(Enum item, string alias = null)
        {
            lock (_syncRoot)
            {
                var enumType = item.GetType();
                alias = alias ?? enumType.Name;
                var enumKey = _keyMapper.GetTranslationKey(enumType, item.ToString(), alias);
                return enumKey;
            }
        }

        public override string ToString()
        {
            return _keyMapper.ToString();
        }
    }
}
