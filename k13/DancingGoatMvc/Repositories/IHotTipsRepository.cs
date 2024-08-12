using System.Collections.Generic;

using CMS.Ecommerce;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for hot tips.
    /// </summary>
    public interface IHotTipsRepository : IRepository
    {
        /// <summary>
        /// Returns collection of products categorized under Hot tips page.
        /// </summary>
        /// <param name="parentAliasPath">Parent node alias path.</param>
        IEnumerable<SKUTreeNode> GetHotTipProducts(string parentAliasPath);
    }
}
