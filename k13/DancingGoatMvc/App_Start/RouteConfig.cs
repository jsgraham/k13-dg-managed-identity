using System.Web.Mvc;
using System.Web.Routing;
using DancingGoat.Infrastructure;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;

namespace DancingGoat
{
    /// <summary>
    /// Provides route configuration for application.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// This is a route controller constraint for pages not handled by the content tree-based router.
        /// The constraint limits the match to a list of specified controllers for pages not handled by the content tree-based router.
        /// The constraint ensures that broken URLs lead to a "404 page not found" page and are not handled by a controller dedicated to the component or 
        /// to a page handled by the content tree-based router (which would lead to an exception).
        /// </summary>
        private const string CONSTRAINT_FOR_NON_ROUTER_PAGE_CONTROLLERS = "Account|Company|Consent|Coupon|Checkout|Media|Navigation|Orders|Search|Subscription";


        /// <summary>
        /// Register custom routes to given <paramref name="routes"/> collection.
        /// </summary>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Uses lowercase URLs for consistency with content tree-based router, and SEO-optimization
            routes.LowercaseUrls = true;

            // Append trailing slash to URLs for consistency with content tree-based router
            routes.AppendTrailingSlash = true;

            // Map routes to Kentico HTTP handlers and features enabled in ApplicationConfig.cs
            // Always map the Kentico routes before adding other routes. Issues may occur if Kentico URLs are matched by a general route, for example images might not be displayed on pages
            routes.Kentico().MapRoutes();

            // Redirect to administration site if the path is "admin"
            routes.MapRoute(
                name: "Admin",
                url: "admin",
                defaults: new { controller = "AdminRedirect", action = "Index" }
            );

            routes.MapRoute(
                name: "ImageUploader",
                url: "Api/{controller}/{action}",
                defaults: new { action = "Upload" },
                constraints: new { controller = "AttachmentImageUploader|MediaFileImageUploader" }
            );

            // Display a custom view for HTTP errors
            routes.MapRoute(
                 name: "HttpErrors",
                 url: "HttpErrors/{action}",
                 defaults: new { controller = "HttpErrors" },
                 constraints: new { culture = new SiteCultureConstraint()}
            );

            routes.MapRoute(
               name: "Default",
               url: "{culture}/{controller}/{action}",
               defaults: new { },
               constraints: new
               {
                   culture = new SiteCultureConstraint { HideLanguagePrefixForDefaultCulture = true },
                   controller = CONSTRAINT_FOR_NON_ROUTER_PAGE_CONTROLLERS
               }
            );

            routes.MapRoute(
               name: "DefaultWithoutLanguagePrefix",
               url: "{controller}/{action}",
               defaults: new { },
               constraints: new
               {
                   culture = new SetDefaultCultureConstraint(),
                   controller = CONSTRAINT_FOR_NON_ROUTER_PAGE_CONTROLLERS
               }
            );

            // Use attribute routing to ensure routes for POST actions in the controllers
            routes.MapMvcAttributeRoutes();
        }
    }
}
