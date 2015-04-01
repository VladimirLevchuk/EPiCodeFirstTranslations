using System;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Creuna.EPiCodeFirstTranslations
{
    public class TranslationContentRenderer<TTranslationContent, TTranslationService>
        where TTranslationService : ITranslationService<TTranslationContent>
    {
        public TranslationContentRenderer(TTranslationService translationService)
        {
            TranslationService = translationService;
        }

        protected virtual TTranslationService TranslationService { get; private set; }

        public IHtmlString Translation(HtmlHelper htmlHelper, Expression<Func<TTranslationContent, string>> translationPathExpression)
        {
            return WriteToHtmlString(w => RenderTranslation(htmlHelper, w, TranslationService.Translate(translationPathExpression)));
        }

        public void RenderTranslation(HtmlHelper htmlHelper, Expression<Func<TTranslationContent, string>> translationPathExpression)
        {
            RenderTranslation(htmlHelper, htmlHelper.ViewContext.Writer, TranslationService.Translate(translationPathExpression));
        }

        public IHtmlString TranslationFormat(HtmlHelper htmlHelper, Expression<Func<TTranslationContent, string>> translationPathExpression, params object[] args)
        {
            return WriteToHtmlString(w => RenderTranslation(htmlHelper, w, TranslationService.Translate(translationPathExpression), t => FormatTranslation(t, args)));
        }

        public void RenderTranslationFormat(HtmlHelper htmlHelper, Expression<Func<TTranslationContent, string>> translationPathExpression, params object[] args)
        {
            RenderTranslation(htmlHelper, htmlHelper.ViewContext.Writer, TranslationService.Translate(translationPathExpression), t => FormatTranslation(t, args));
        }

        protected virtual string FormatTranslation(string translation, object[] args)
        {
            return string.Format(translation, args);
        }

        public IHtmlString TranslationForEnumValue(HtmlHelper htmlHelper, Enum value)
        {
            return WriteToHtmlString(w => RenderTranslation(htmlHelper, w, TranslationService.TranslateEnumValue(value)));
        }

        public void RenderTranslationForEnumValue(HtmlHelper htmlHelper, Enum value)
        {
            RenderTranslation(htmlHelper, htmlHelper.ViewContext.Writer, TranslationService.TranslateEnumValue(value));
        }

        protected IHtmlString WriteToHtmlString(Action<TextWriter> writeAction)
        {
            using (var writer = new StringWriter(CultureInfo.CurrentCulture))
            {
                writeAction(writer);
                return MvcHtmlString.Create(writer.ToString());
            }
        }

        protected virtual void RenderTranslation(HtmlHelper htmlHelper, TextWriter writer, string translation, Func<string, string> prepareToRenderFunction = null)
        {
            if (htmlHelper == null) throw new ArgumentNullException("htmlHelper");

            if (prepareToRenderFunction != null)
            {
                translation = prepareToRenderFunction(translation);
            }

            writer.Write(translation);
        }
    }
}
