using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Routing;

using CMS.Helpers;
using CMS.SiteProvider;

namespace DancingGoat.Infrastructure
{
    /// <summary>
    /// Constraint for a route that handles URLs with hidden language prefix in default culture.
    /// </summary>
    public class SetDefaultCultureConstraint : IRouteConstraint
    {
        /// <summary>
        /// Propagates default culture into current thread.
        /// </summary>
        /// <param name="httpContext">Object that encapsulates information about the HTTP request.</param>
        /// <param name="route">Object that this constraint belongs to.</param>
        /// <param name="parameterName">Name of the parameter that is being checked.</param>
        /// <param name="values">Object that contains the parameters for the URL.</param>
        /// <param name="routeDirection">Object that indicates whether the constraint check is being performed when an incoming request is being handled or when a URL is being generated.</param>
        /// <returns>The constraint always matches, returns <c>true</c>.</returns>
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            var defaultCultureCode = CultureHelper.GetDefaultCultureCode(SiteContext.CurrentSiteName);
            var culture = new CultureInfo(defaultCultureCode);

            Thread.CurrentThread.CurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;

            return true; 
        }
    }
}