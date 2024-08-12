using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Ecommerce;
using CMS.Helpers;
using CMS.SiteProvider;
using DancingGoat.Infrastructure;
using Kentico.Content.Web.Mvc;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Encapsulates access to hot tips.
    /// </summary>
    public class KenticoHotTipsRepository : IHotTipsRepository
    {
        private readonly IPageRetriever pageRetriever;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoHotTipsRepository"/> class that returns hot tips.
        /// </summary>
        /// <param name="pageRetriever">Retriever for pages based on given parameters.</param>
        public KenticoHotTipsRepository(IPageRetriever pageRetriever)
        {
            this.pageRetriever = pageRetriever;
        }


        /// <summary>
        /// Returns collection of products categorized under Hot tips page.
        /// </summary>
        /// <param name="parentAliasPath">Parent node alias path.</param>
        public IEnumerable<SKUTreeNode> GetHotTipProducts(string parentAliasPath)
        {
            return RepositoryCacheHelper.CachePages(() =>
            {
                var hotTipsPage = pageRetriever.Retrieve<HotTips>(
                    query => query
                        .Path($"{parentAliasPath}/Hot-tips", PathTypeEnum.Single)
                        .TopN(1))
                    .FirstOrDefault();

                return hotTipsPage?.Fields.HotTips
                    .OfType<SKUTreeNode>() ?? Enumerable.Empty<SKUTreeNode>();
            }, $"{nameof(KenticoHotTipsRepository)}|{nameof(GetHotTipProducts)}|{parentAliasPath}", new[] 
            {
                CacheDependencyKeyProvider.GetDependencyCacheKeyForObjectType("cms.adhocrelationship"),
                CacheDependencyKeyProvider.GetDependencyCacheKeyForObjectType("ecommerce.sku"), 
                $"node|{SiteContext.CurrentSiteName}|{parentAliasPath}"
            });
        }
    }
}