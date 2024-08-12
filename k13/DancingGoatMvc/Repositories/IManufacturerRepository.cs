using System.Collections.Generic;

using CMS.DocumentEngine.Types.DancingGoatMvc;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for manufacturers.
    /// </summary>
    public interface IManufacturerRepository : IRepository
    {
        /// <summary>
        /// Returns manufacturers from stored the content tree.
        /// </summary>
        /// <param name="parentAliasPath">Parent node alias path.</param>
        IEnumerable<Manufacturer> GetManufacturers(string parentAliasPath);
    }
}