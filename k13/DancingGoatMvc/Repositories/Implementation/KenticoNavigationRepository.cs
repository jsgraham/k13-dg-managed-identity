using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.SiteProvider;

using DancingGoat.Infrastructure;

using Kentico.Content.Web.Mvc;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of navigation items.
    /// </summary>
    public class KenticoNavigationRepository : INavigationRepository
    {
        private readonly IPageRetriever pageRetriever;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoNavigationRepository"/> class that returns navigation items.
        /// </summary>
        /// <param name="pageRetriever">Retriever for pages based on given parameters.</param>
        public KenticoNavigationRepository(IPageRetriever pageRetriever)
        {
            this.pageRetriever = pageRetriever;
        }


        /// <summary>
        /// Returns an enumerable collection of menu items ordered by the content tree order and level.
        /// </summary>
        public IEnumerable<TreeNode> GetMenuItems()
        {
            return pageRetriever.Retrieve<TreeNode>(
                query => query
                    .FilterDuplicates()
                    .OrderByAscending("NodeLevel", "NodeOrder")
                    .MenuItems(),
                cache => cache
                    .Key($"{nameof(KenticoNavigationRepository)}|{nameof(GetMenuItems)}")
                    // Include path dependency to flush cache when a new child page is created or page order is changed.
                    .Dependencies((_, builder) => builder.PagePath("/", PathTypeEnum.Children).ObjectType("cms.documenttype").PageOrder()));
        }


        /// <summary>
        /// Returns an enumerable collection of footer navigation items.
        /// </summary>
        public IEnumerable<TreeNode> GetFooterNavigationItems()
        {
            return RepositoryCacheHelper.CachePages(() =>
            {
                var footerNavigationPage = pageRetriever.Retrieve<FooterNavigation>(
                    query => query
                        .Path(ContentItemIdentifiers.FOOTER_NAVIGATION, PathTypeEnum.Single)
                        .TopN(1))
                    .First();

                return footerNavigationPage.Fields.NavigationItems;
            }, $"{nameof(KenticoNavigationRepository)}|{nameof(GetFooterNavigationItems)}", new[] 
            {
                CacheDependencyKeyProvider.GetDependencyCacheKeyForObjectType("cms.adhocrelationship"), 
                $"node|{SiteContext.CurrentSiteName}|/|childnodes"
            });
        }
    }
}