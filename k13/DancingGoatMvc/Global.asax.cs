using System;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;

using CMS.ContactManagement;
using CMS.DataEngine;

using Kentico.OnlineMarketing.Web.Mvc;
using Kentico.Web.Mvc;

namespace DancingGoat
{
    /// <summary>
    /// Describes the application.
    /// </summary>
    public class DancingGoatApplication : HttpApplication
    {
        /// <summary>
        /// Occurs when application starts.
        /// </summary>
        protected void Application_Start()
        {
            // Enable and configure selected Kentico ASP.NET MVC integration features
            ApplicationConfig.RegisterFeatures(ApplicationBuilder.Current);

            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Register routes including system routes for enabled features
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Registers implementation to the dependency resolver
            DependencyResolverConfig.Register();
        }


        /// <summary>
        /// Occurs when an unhandled exception in application is thrown.
        /// </summary>
        protected void Application_Error()
        {
            LastErrorLogger.LogLastApplicationError();
            Handle404Error();
        }




        /// <summary>
        /// Overrides basic application-wide implementation of the <see cref="P:System.Web.UI.PartialCachingAttribute.VaryByCustom" /> property.
        /// </summary>
        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            var options = OutputCacheKeyHelper.CreateOptions();
            var contactGroupKey = String.Empty;

            switch (custom)
            {
                case "DefaultProfile":
                    options
                        .VaryByUser()
                        .VaryByHost()
                        .VaryByCookieLevel();
                    break;

                case "PageBuilderProfile":
                    options
                        .VaryByUser()
                        .VaryByHost()
                        .VaryByPersona()
                        .VaryByABTestVariant()
                        .VaryByCookieLevel();

                    contactGroupKey = GetContactGroupCacheKey();
                    break;
            }

            // Get cache key generated based on cache options
            var builtCacheKey = OutputCacheKeyHelper.GetVaryByCustomString(context, custom, options);

            // Combine cache options with custom contact group cache key if exists
            if (!String.IsNullOrEmpty(contactGroupKey))
            {
                builtCacheKey = String.Join("|", builtCacheKey, contactGroupKey);
            }

            if (!String.IsNullOrEmpty(builtCacheKey))
            {
                return builtCacheKey;
            }

            return base.GetVaryByCustomString(context, custom);
        }


        /// <summary>
        /// Gets the cache key with current contant contact groups
        /// </summary>
        private string GetContactGroupCacheKey()
        {
            // Gets the current contact, without creating a new anonymous contact for new visitors
            var existingContact = ContactManagementContext.GetCurrentContact(createAnonymous: false);
            var groupsKey = String.Empty;

            if (existingContact != null)
            {
                var groups = existingContact?.ContactGroups?.Select(x => x.ContactGroupID).OrderBy(x => x).ToArray() ?? new int[] { };
                groupsKey = String.Join(";", groups);
            }

            return $"ContactGroup={groupsKey}";
        }

        /// <summary>
        /// Handles the 404 error and setups the response for being handled by IIS (IIS behavior is specified in the "httpErrors" section in the web.config file).
        /// </summary>
        private void Handle404Error()
        {
            var error = Server.GetLastError();
            if ((error as HttpException)?.GetHttpCode() == 404)
            {
                Server.ClearError();
                Response.StatusCode = 404;
            }
        }
    }
}