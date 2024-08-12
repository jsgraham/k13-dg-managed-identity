using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

using CMS.Core;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.Membership;

using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

namespace DancingGoat.Helpers
{
    /// <summary>
    /// Extension methods for <see cref="HtmlHelper{TModel}"/> class.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Returns an HTML input element with a label and validation fields for each property in the object that is represented by the <see cref="Expression"/> expression.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="expression">An expression that identifies the object that contains the displayed properties.</param>
        /// <param name="explanationText">An explanation text describing usage of the rendered field.</param>
        /// <param name="disabled">Indicates that field has to be disabled.</param>
        public static MvcHtmlString ValidatedEditorFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string explanationText = "", bool disabled = false)
        {
            var label = html.LabelFor(expression).ToString();
            var additionalViewData = disabled ? new { htmlAttributes = new { disabled = "disabled" } } : null;
            var editor = html.EditorFor(expression, additionalViewData).ToString();
            var message = html.ValidationMessageFor(expression).ToString();
            var explanationTextHtml = "";

            if (!string.IsNullOrEmpty(explanationText))
            {
                explanationTextHtml = "<div class=\"explanation-text\">" + explanationText + "</div>";
            }

            var generatedHtml = string.Format(@"
<div class=""form-group"">
    <div class=""form-group-label"">{0}</div>
    <div class=""form-group-input"">{1}
       {2}
    </div>
    <div class=""message message-error"">{3}</div>
</div>", label, editor, explanationTextHtml, message);

            return MvcHtmlString.Create(generatedHtml);
        }


        /// <summary>
        /// Generates an A tag with a "mailto" link.
        /// </summary>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <param name="email">Email address.</param>
        public static MvcHtmlString MailTo(this HtmlHelper htmlHelper, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return MvcHtmlString.Empty;
            }

            var link = string.Format("<a href=\"mailto:{0}\">{1}</a>", HTMLHelper.EncodeForHtmlAttribute(email), HTMLHelper.HTMLEncode(email));

            return MvcHtmlString.Create(link);
        }


        /// <summary>
        /// Generates a link for an actual page with the specified culture.
        /// </summary>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <param name="linkText">Displayed text.</param>
        /// <param name="cultureName">Name of the new culture.</param>
        /// <returns>RouteLink for the given culture.</returns>
        public static MvcHtmlString CultureLink(this HtmlHelper htmlHelper, string linkText, string cultureName)
        {
            // Page data context is initialized
            if (Service.Resolve<IPageDataContextRetriever>().TryRetrieve<TreeNode>(out var _))
            {
                var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext);
                var url = urlHelper.Kentico().CurrentPageUrl(cultureName).ToAbsolute();

                return htmlHelper.HtmlLink(url, linkText);
            }

            var queryParams = htmlHelper.ViewContext.HttpContext.Request.Unvalidated.QueryString;
            var originalRouteValues = htmlHelper.ViewContext.RouteData.Values;

            // Create a link for the current culture (the URL stays as it is)
            if ((string)originalRouteValues["culture"] == cultureName)
            {
                return htmlHelper.HtmlLink(htmlHelper.ViewContext.HttpContext.Request.RawUrl, linkText);
            }

            // Clone the original route information
            var newRouteValues = new RouteValueDictionary(originalRouteValues);            

            // Add query parameters (e.g. when performing a search)
            foreach (string key in queryParams)
            {
                // Remove the key from the collection before adding a new value to prevent throwing an exception
                if (!String.IsNullOrEmpty(key))
                {
                    newRouteValues[key] = queryParams[key];
                }
            }

            // Ensure correct culture prefix
            newRouteValues["culture"] = cultureName;

            return htmlHelper.RouteLink(linkText, newRouteValues);
        }


        /// <summary>
        /// Generates an A tag with a link to the specified URL.
        /// </summary>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <param name="url">Link URL.</param>
        /// <param name="linkText">Displayed text.</param>
        /// <param name="htmlAttributes">Additional tag attributes.</param>
        public static MvcHtmlString HtmlLink(this HtmlHelper htmlHelper, string url, string linkText, object htmlAttributes = null)
        {
            var link = new TagBuilder("a");
            link.InnerHtml = linkText;
            link.MergeAttributes(new RouteValueDictionary(htmlAttributes));
            link.MergeAttribute("href", url);

            return MvcHtmlString.Create(link.ToString());
        }


        /// <summary>
        /// Returns localization for given resource string key in a preferred UI culture of current user.
        /// </summary>
        /// <param name="htmlHelper">HTML helper.</param>
        /// <param name="key">Resource string key.</param>
        /// <remarks>Returned string is not HTML encoded.</remarks>
        public static IHtmlString GetUIString(this HtmlHelper htmlHelper, string key)
        {
            return htmlHelper.Raw(ResHelper.GetString(key, MembershipContext.AuthenticatedUser.PreferredUICultureCode));
        }
    }
}