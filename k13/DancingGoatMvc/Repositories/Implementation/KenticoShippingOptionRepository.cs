using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Ecommerce;
using CMS.SiteProvider;

using DancingGoat.Infrastructure;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Provides CRUD operations for shipping options.
    /// </summary>
    public class KenticoShippingOptionRepository : IShippingOptionRepository
    {
        private readonly IShippingOptionInfoProvider shippingOptionInfoProvider;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoShippingOptionRepository"/> class using the shipping option provider given.
        /// </summary>
        /// <param name="shippingOptionInfoProvider">Provider for <see cref="ShippingOptionInfo"/> management.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="shippingOptionInfoProvider"/> is null.</exception>
        public KenticoShippingOptionRepository(IShippingOptionInfoProvider shippingOptionInfoProvider)
        {
            this.shippingOptionInfoProvider = shippingOptionInfoProvider ?? throw new ArgumentNullException(nameof(shippingOptionInfoProvider));
        }


        /// <summary>
        /// Returns a shipping option with the specified identifier.
        /// </summary>
        /// <param name="shippingOptionId">Shipping option's identifier.</param>
        /// <returns><see cref="ShippingOptionInfo"/> object representing a shipping option with the specified identifier. Returns <c>null</c> if not found.</returns>
        public ShippingOptionInfo GetById(int shippingOptionId)
        {
            return RepositoryCacheHelper.CacheObject(() =>
            {
                var shippingInfo = shippingOptionInfoProvider.Get(shippingOptionId);

                if (shippingInfo == null || shippingInfo.ShippingOptionSiteID != SiteContext.CurrentSiteID)
                {
                    return null;
                }

                return shippingInfo;
            }, $"{nameof(KenticoShippingOptionRepository)}|{nameof(GetById)}|{shippingOptionId}");
        }


        /// <summary>
        /// Returns an enumerable collection of all enabled shipping options.
        /// </summary>
        /// <returns>Collection of enabled shipping options. See <see cref="ShippingOptionInfo"/> for detailed information.</returns>
        public IEnumerable<ShippingOptionInfo> GetAllEnabled()
        {
            return RepositoryCacheHelper.CacheObjects(() =>
            {
                return shippingOptionInfoProvider.GetBySite(SiteContext.CurrentSiteID, true);
            }, $"{nameof(KenticoShippingOptionRepository)}|{nameof(GetAllEnabled)}");
        }
    }
}