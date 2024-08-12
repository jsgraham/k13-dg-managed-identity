using System;
using System.Collections.Generic;
using System.Linq;

using CMS.Ecommerce;
using CMS.SiteProvider;

using DancingGoat.Infrastructure;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Provides CRUD operations for orders.
    /// </summary>
    public class KenticoOrderRepository : IOrderRepository
    {
        private readonly IOrderInfoProvider orderInfoProvider;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoOrderRepository"/> class.
        /// </summary>
        /// <param name="orderInfoProvider">Provider for <see cref="OrderInfo"/> management.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="orderInfoProvider"/> is null.</exception>
        public KenticoOrderRepository(IOrderInfoProvider orderInfoProvider)
        {
            this.orderInfoProvider = orderInfoProvider ?? throw new ArgumentNullException(nameof(orderInfoProvider));
        }


        /// <summary>
        /// Returns an order with the specified identifier.
        /// </summary>
        /// <param name="orderId">Order's identifier.</param>
        /// <returns><see cref="OrderInfo"/> object representing an order with the specified identifier. Returns <c>null</c> if not found.</returns>
        public OrderInfo GetById(int orderId)
        {
            return RepositoryCacheHelper.CacheObject(() =>
            {
                var orderInfo = orderInfoProvider.Get(orderId);

                if (orderInfo == null || orderInfo.OrderSiteID != SiteContext.CurrentSiteID)
                {
                    return null;
                }

                return orderInfo;
            }, $"{nameof(KenticoOrderRepository)}|{nameof(GetById)}|{orderId}");
        }


        /// <summary>
        /// Returns an enumerable collection of TopN orders of the given customer ordered by OrderDate descending.
        /// </summary>
        /// <param name="customerId">Customer's identifier.</param>
        /// <param name="count">Number of retrieved orders. Using 0 returns all records.</param>
        /// <returns>Collection of the customer's orders. See <see cref="OrderInfo"/> for detailed information.</returns>
        public IEnumerable<OrderInfo> GetByCustomerId(int customerId, int count = 0)
        {
            return RepositoryCacheHelper.CacheObjects(() =>
            {
                return orderInfoProvider.GetBySite(SiteContext.CurrentSiteID)
                    .WhereEquals("OrderCustomerID", customerId)
                    .TopN(count)
                    .OrderByDescending(orderInfo => orderInfo.OrderDate);
            }, $"{nameof(KenticoOrderRepository)}|{nameof(GetByCustomerId)}|{customerId}|{count}");
        }
    }
}