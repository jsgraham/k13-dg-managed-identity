using System;
using System.Collections.Generic;

using CMS.DocumentEngine.Types.DancingGoatMvc;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of cafes.
    /// </summary>
    public interface ICafeRepository : IRepository
    {
        /// <summary>
        /// Returns an enumerable collection of company cafes ordered by a position in the content tree.
        /// </summary>
        /// <param name="nodeAliasPath">The node alias path of the articles section in the content tree.</param>
        /// <param name="count">The number of cafes to return. Use 0 as value to return all records.</param>
        IEnumerable<Cafe> GetCompanyCafes(string nodeAliasPath, int count = 0);


        /// <summary>
        /// Returns an enumerable collection of partner cafes ordered by a position in the content tree.
        /// </summary>
        /// <param name="nodeAliasPath">The node alias path of the articles section in the content tree.</param>
        IEnumerable<Cafe> GetPartnerCafes(string nodeAliasPath);


        /// <summary>
        /// Returns a single cafe for the given <paramref name="guid"/>.
        /// </summary>
        /// <param name="guid">Node Guid.</param>
        Cafe GetCafeByGuid(Guid guid);
    }
}