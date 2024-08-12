using CMS.Ecommerce;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of public statuses.
    /// </summary>
    public interface IPublicStatusRepository : IRepository
    {
        /// <summary>
        /// Returns a public status with the specified name.
        /// </summary>
        /// <param name="name">The code name of the public status.</param>
        PublicStatusInfo GetByName(string name);


        /// <summary>
        /// Returns a public status with the specified id.
        /// </summary>
        /// <param name="statusId">The id of the public status.</param>
        PublicStatusInfo GetById(int statusId);
    }
}