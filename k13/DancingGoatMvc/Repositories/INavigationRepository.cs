using System.Collections.Generic;
using CMS.DocumentEngine;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of navigation items.
    /// </summary>
    public interface INavigationRepository : IRepository
    {
        /// <summary>
        /// Returns an enumerable collection of menu items ordered by the content tree order and level.
        /// </summary>
        /// <returns>An enumerable collection that contains the menu items.</returns>
        IEnumerable<TreeNode> GetMenuItems();


        /// <summary>
        /// Returns an enumerable collection of footer navigation items.
        /// </summary>
        /// <returns>An enumerable collection that contains the footer navigation items.</returns>
        IEnumerable<TreeNode> GetFooterNavigationItems();
    }
}