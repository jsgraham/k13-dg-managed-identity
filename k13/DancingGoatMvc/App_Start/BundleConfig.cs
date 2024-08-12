using System.Web.Optimization;

namespace DancingGoat
{
    /// <summary>
    /// Bundle configuration for application.
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Register bundles to given <paramref name="bundles"/> collection.
        /// </summary>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            RegisterJqueryBundle(bundles);
            RegisterJqueryUnobtrusiveAjaxBundle(bundles);
            RegisterJqueryValidationBundle(bundles);
        }


        private static void RegisterJqueryBundle(BundleCollection bundles)
        {
            var bundle = new ScriptBundle("~/bundles/jquery")
            {
                CdnPath = "//ajax.aspnetcdn.com/ajax/jQuery/jquery-3.5.1.min.js",
                CdnFallbackExpression = "window.jQuery"
            };

            bundle.Include("~/Scripts/jquery-{version}.js");

            bundles.Add(bundle);
        }


        private static void RegisterJqueryUnobtrusiveAjaxBundle(BundleCollection bundles)
        {
            var bundle = new ScriptBundle("~/bundles/jquery-unobtrusive-ajax")
                .Include("~/Scripts/jquery.unobtrusive-ajax.js");

            bundles.Add(bundle);
        }


        private static void RegisterJqueryValidationBundle(BundleCollection bundles)
        {
            var bundle = new ScriptBundle("~/bundles/jquery-validation")
                .Include("~/Scripts/jquery.validate*");

            bundles.Add(bundle);
        }
    }
}