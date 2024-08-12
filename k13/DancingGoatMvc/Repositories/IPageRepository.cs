using System;

using CMS.DocumentEngine;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for collection of pages.
    /// </summary>
    public interface IPageRepository : IRepository
    {
        /// <summary>
        /// Returns a page with the specified identifier.
        /// </summary>
        /// <param name="nodeGuid">The page node identifier.</param>
        /// <returns>The page with the specified node identifier; null if not found.</returns>
        TreeNode Get(Guid nodeGuid);
    }
}
