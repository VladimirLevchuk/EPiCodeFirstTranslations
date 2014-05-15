using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Creuna.EPiCodeFirstTranslations;
using EPiServer.ServiceLocation;

namespace Creuna.EPiCodeFirstTranslations.Presentation.Translation
{
    public static class TranslationExtentions
    {
        private static TranslationContentRenderer<Translations, TranslationService> TranslationContentRenderer
        {
            get { return ServiceLocator.Current.GetInstance<TranslationContentRenderer<Translations, TranslationService>>(); }
        }

        public static IHtmlString Translation(this HtmlHelper htmlHelper, Expression<Func<Translations, string>> translationPathExpression)
        {
            return TranslationContentRenderer.Translation(htmlHelper, translationPathExpression);
        }

        public static void RenderTranslation(this HtmlHelper htmlHelper, Expression<Func<Translations, string>> translationPathExpression)
        {
            TranslationContentRenderer.RenderTranslation(htmlHelper, translationPathExpression);
        }

        public static IHtmlString TranslationFormat(this HtmlHelper htmlHelper, Expression<Func<Translations, string>> translationPathExpression, params object[] args)
        {
            return TranslationContentRenderer.TranslationFormat(htmlHelper, translationPathExpression, args);
        }

        public static void RenderTranslationFormat(this HtmlHelper htmlHelper, Expression<Func<Translations, string>> translationPathExpression, params object[] args)
        {
            TranslationContentRenderer.RenderTranslationFormat(htmlHelper, translationPathExpression, args);
        }
    }
}