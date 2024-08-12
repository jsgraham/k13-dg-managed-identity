using System.Web.Mvc;

using CMS.Core;

namespace DancingGoat.Controllers.FormSections
{
    /// <summary>
    /// Attribute handling errors occurring during form sections rendering.
    /// </summary>
    public sealed class FormSectionExceptionFilterAttribute : FilterAttribute, IExceptionFilter
    {
        /// <summary>
        /// Logs exception to the event log. Responds with 200 status code and no markup.
        /// </summary>
        /// <param name="exceptionContext">Provides the exception context.</param>
        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
            {
                return;
            }

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.StatusCode = 200;

            Service.Resolve<IEventLogService>().LogException("FormSection", filterContext.RouteData.Values["action"]?.ToString(), filterContext.Exception);
        }
    }
}
