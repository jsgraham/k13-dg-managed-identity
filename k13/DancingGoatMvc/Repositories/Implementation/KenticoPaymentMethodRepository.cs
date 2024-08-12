using System;
using System.Collections.Generic;

using CMS.Ecommerce;
using CMS.SiteProvider;

using DancingGoat.Infrastructure;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Provides CRUD operations for payment methods.
    /// </summary>
    public class KenticoPaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly IPaymentOptionInfoProvider paymentOptionInfoProvider;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoPaymentMethodRepository"/> class using the payment option provider given.
        /// </summary>
        /// <param name="paymentOptionInfoProvider">Provider for <see cref="PaymentOptionInfo"/> management.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="paymentOptionInfoProvider"/> is null.</exception>
        public KenticoPaymentMethodRepository(IPaymentOptionInfoProvider paymentOptionInfoProvider)
        {
            this.paymentOptionInfoProvider = paymentOptionInfoProvider ?? throw new ArgumentNullException(nameof(paymentOptionInfoProvider));
        }


        /// <summary>
        /// Returns a payment method with the specified identifier.
        /// </summary>
        /// <param name="paymentMethodId">Payment method's identifier.</param>
        /// <returns><see cref="PaymentOptionInfo"/> object representing a payment method with the specified identifier. Returns <c>null</c> if not found.</returns>
        public PaymentOptionInfo GetById(int paymentMethodId)
        {
            return RepositoryCacheHelper.CacheObject(() =>
            {
                var paymentInfo = paymentOptionInfoProvider.Get(paymentMethodId);

                if (paymentInfo?.PaymentOptionSiteID == SiteContext.CurrentSiteID)
                {
                    return paymentInfo;
                }

                if (paymentInfo?.PaymentOptionSiteID == 0 && ECommerceSettings.AllowGlobalPaymentMethods(SiteContext.CurrentSiteID))
                {
                    return paymentInfo;
                }

                return null;
            }, $"{nameof(KenticoPaymentMethodRepository)}|{nameof(GetById)}|{paymentMethodId}");
        }


        /// <summary>
        /// Returns an enumerable collection of all enabled payment methods.
        /// </summary>
        /// <returns>Collection of enabled payment methods. See <see cref="PaymentOptionInfo"/> for detailed information.</returns>
        public IEnumerable<PaymentOptionInfo> GetAll()
        {
            return RepositoryCacheHelper.CacheObjects(() =>
            {
                return paymentOptionInfoProvider.GetBySite(SiteContext.CurrentSiteID, true);
            }, $"{nameof(KenticoPaymentMethodRepository)}|{nameof(GetAll)}");
        }
    }
}