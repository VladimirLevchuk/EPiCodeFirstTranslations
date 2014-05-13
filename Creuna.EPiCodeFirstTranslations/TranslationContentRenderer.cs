using System;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Creuna.EPiCodeFirstTranslations
{
    public class TranslationContentRenderer<TTranslationContent, TTranslationService>
        where TTranslationContent : ITranslationContent
        where TTranslationService : ITranslationService<TTranslationContent>
    {
        public TranslationContentRenderer(TTranslationService translationService)
        {
            TranslationService = translationService;
        }

        protected virtual TTranslationService TranslationService { get; private set; }

        public IHtmlString Translation(HtmlHelper htmlHelper, Expression<Func<TTranslationContent, string>> translationPathExpression)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException("htmlHelper");
            }

            if (translationPathExpression == null)
            {
                throw new ArgumentNullException("translationPathExpression");
            }

            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                RenderTranslation(htmlHelper, translationPathExpression, writer);
                return MvcHtmlString.Create(writer.ToString());
            }
        }

        public void RenderTranslation(HtmlHelper htmlHelper, Expression<Func<TTranslationContent, string>> translationPathExpression)
        {
            if (htmlHelper == null)
            {
                throw new ArgumentNullException("htmlHelper");
            }

            if (translationPathExpression == null)
            {
                throw new ArgumentNullException("translationPathExpression");
            }

            RenderTranslation(htmlHelper, translationPathExpression, htmlHelper.ViewContext.Writer);
        }

        protected virtual void RenderTranslation(HtmlHelper htmlHelper, Expression<Func<TTranslationContent, string>> translationPathExpression, TextWriter writer)
        {
            writer.Write(TranslationService.Translate(translationPathExpression));
        }
    }
}
