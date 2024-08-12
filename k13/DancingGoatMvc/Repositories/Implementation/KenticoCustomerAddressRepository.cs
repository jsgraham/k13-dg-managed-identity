using System;
using System.Collections.Generic;

using CMS.Ecommerce;

using DancingGoat.Infrastructure;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Provides CRUD operations for customer addresses.
    /// </summary>
    public class KenticoCustomerAddressRepository : ICustomerAddressRepository
    {
        private readonly IAddressInfoProvider addressInfoProvider;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoCustomerAddressRepository"/> class using the address provider given.
        /// </summary>
        /// <param name="addressInfoProvider">Provider for <see cref="AddressInfo"/> management.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="addressInfoProvider"/> is null.</exception>
        public KenticoCustomerAddressRepository(IAddressInfoProvider addressInfoProvider)
        {
            this.addressInfoProvider = addressInfoProvider ?? throw new ArgumentNullException(nameof(addressInfoProvider));
        }


        /// <summary>
        /// Returns a customer's address with the specified identifier.
        /// </summary>
        /// <param name="addressId">Identifier of the customer's address.</param>
        /// <returns>Customer's address with the specified identifier. Returns <c>null</c> if not found.</returns>
        public AddressInfo GetById(int addressId)
        {
            return RepositoryCacheHelper.CacheObject(() =>
            {
                return addressInfoProvider.Get(addressId);
            }, $"{nameof(KenticoCustomerAddressRepository)}|{nameof(GetById)}|{addressId}");
        }


        /// <summary>
        /// Returns an enumerable collection of a customer's addresses.
        /// </summary>
        /// <param name="customerId">Customer's identifier.</param>
        /// <returns>Collection of customer's addresses. See <see cref="AddressInfo"/> for detailed information.</returns>
        public IEnumerable<AddressInfo> GetByCustomerId(int customerId)
        {
            return RepositoryCacheHelper.CacheObjects(() =>
            {
                return addressInfoProvider.GetByCustomer(customerId);
            }, $"{nameof(KenticoCustomerAddressRepository)}|{nameof(GetByCustomerId)}|{customerId}");
        }


        /// <summary>
        /// Saves a customer's address into the database.
        /// </summary>
        /// <param name="address"><see cref="AddressInfo"/> object representing a customer's address that is inserted.</param>
        public void Upsert(AddressInfo address)
        {
            addressInfoProvider.Set(address);
        }
    }
}