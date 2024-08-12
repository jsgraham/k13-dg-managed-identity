using DancingGoat.PageTemplatesFilters;

using Kentico.Activities.Web.Mvc;
using Kentico.CampaignLogging.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;
using Kentico.Newsletters.Web.Mvc;
using Kentico.OnlineMarketing.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.Scheduler.Web.Mvc;
using Kentico.Web.Mvc;

namespace DancingGoat
{
    /// <summary>
    /// Class for application configuration.
    /// </summary>
    public class ApplicationConfig
    {
        /// <summary>
        /// Registers features into given <paramref name="builder"/>.
        /// </summary>
        public static void RegisterFeatures(IApplicationBuilder builder)
        {
            builder.UsePageBuilder(new PageBuilderOptions
            {
                DefaultSectionIdentifier = ComponentIdentifiers.SINGLE_COLUMN_SECTION,
                RegisterDefaultSection = false
            });

            builder.UseDataAnnotationsLocalization();
            builder.UseCampaignLogger();
            builder.UseActivityTracking();
            builder.UseABTesting();
            builder.UseWebAnalytics();
            builder.UseEmailTracking();
            builder.UseScheduler();
            builder.UsePageRouting(new PageRoutingOptions
            {
                EnableAlternativeUrls = true,
                CultureCodeRouteValuesKey = "culture"
            });

            builder.UseSmartSearchIndexRebuild();

            // Register page template filers into the global page template filter collection.
            PageBuilderFilters.PageTemplates.Add(new LandingPageTemplatesFilter());
            PageBuilderFilters.PageTemplates.Add(new ArticlePageTemplatesFilter());
        }
    }
}
