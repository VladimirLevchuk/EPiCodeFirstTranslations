using System;
using System.Collections.Generic;
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
            var propertyPathString = ExpressionUtils.GetPropertyPathString(translationPath);
            return GetTranslationKey(propertyPathString);
        }

        protected virtual string GetTranslationKey(string propertyPathString)
        {
            lock (_syncRoot)
            {
                var translationKey = _keyMapper.GetTranslationKey(typeof(TTranslationContent), propertyPathString);
                return translationKey;
            }
        }

        public virtual string GetTranslationKey(IEnumerable<string> propertyPath)
        {
            var propertyPathString = ExpressionUtils.GetPropertyPathString(propertyPath);
            return GetTranslationKey(propertyPathString);
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
            lock (_syncRoot)
            {
                return _keyMapper.ToString();
            }
        }
    }
}
