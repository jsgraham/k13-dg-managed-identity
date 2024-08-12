using System;
using System.Web;
using System.Web.Mvc;

using CMS.Helpers;
using CMS.SiteProvider;

using DancingGoat;

using Kentico.Membership;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;

using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace DancingGoat
{
    /// <summary>
    /// Wraps application authentication configuration.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The application authentication cookie name
        /// </summary>
        private const string AUTHENTICATION_COOKIE_NAME = "owin.authentication";


        /// <summary>
        /// Configures the application authentication.
        /// </summary>
        public void Configuration(IAppBuilder app)
        {
            // Register Kentico Membership identity implementation
            app.CreatePerOwinContext(() => KenticoUserManager.Initialize(app, new KenticoUserManager(new KenticoUserStore(SiteContext.CurrentSiteName))));
            app.CreatePerOwinContext<KenticoSignInManager>(KenticoSignInManager.Create);

            // Configure the sign in cookie
            UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<KenticoUserManager, User, int>(
                        // Sets the interval after which the validity of the user's security stamp is checked
                        validateInterval: TimeSpan.FromMinutes(1),
                        regenerateIdentityCallback: (manager, user) => manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie),
                        getUserIdCallback: ((claimsIdentity) => int.Parse(claimsIdentity.GetUserId()))),
                    // Redirect to logon page with return url
                    OnApplyRedirect = context => context.Response.Redirect(urlHelper.Action("Login", "Account") + new Uri(context.RedirectUri).Query)
                },
                ExpireTimeSpan = TimeSpan.FromDays(14),
                SlidingExpiration = true,
                CookieName = AUTHENTICATION_COOKIE_NAME
            });

            // Register the authentication cookie in the Kentico application and set its cookie level.
            // This will ensure that the authentication cookie will not be removed when a user revokes the tracking consent.
            CookieHelper.RegisterCookie(AUTHENTICATION_COOKIE_NAME, CookieLevel.Essential);
        }
    }
}
