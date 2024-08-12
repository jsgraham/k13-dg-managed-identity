using System;

using CMS.Ecommerce;
using CMS.SiteProvider;

using DancingGoat.Infrastructure;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a contract for a collection of public statuses.
    /// </summary>
    public class KenticoPublicStatusRepository : IPublicStatusRepository
    {
        private readonly IPublicStatusInfoProvider publicStatusInfoProvider;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoPublicStatusRepository"/> class using the public status provider given.
        /// </summary>
        /// <param name="publicStatusInfoProvider">Provider for <see cref="PublicStatusInfo"/> management.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="publicStatusInfoProvider"/> is null.</exception>
        public KenticoPublicStatusRepository(IPublicStatusInfoProvider publicStatusInfoProvider)
        {
            this.publicStatusInfoProvider = publicStatusInfoProvider ?? throw new ArgumentNullException(nameof(publicStatusInfoProvider));
        }


        /// <summary>
        /// Returns a public status with the specified name.
        /// </summary>
        /// <param name="name">The code name of the public status.</param>
        public PublicStatusInfo GetByName(string name)
        {
            return RepositoryCacheHelper.CacheObject(() =>
            {
                return publicStatusInfoProvider.Get(name, SiteContext.CurrentSiteID);
            }, $"{nameof(KenticoPublicStatusRepository)}|{nameof(GetByName)}|{name}");
        }


        /// <summary>
        /// Returns a public status with the specified id.
        /// </summary>
        /// <param name="statusId">The id of the public status.</param>
        public PublicStatusInfo GetById(int statusId)
        {
            return RepositoryCacheHelper.CacheObject(() =>
            {
                return publicStatusInfoProvider.Get(statusId);
            }, $"{nameof(KenticoPublicStatusRepository)}|{nameof(GetById)}|{statusId}");
        }
    }
}
