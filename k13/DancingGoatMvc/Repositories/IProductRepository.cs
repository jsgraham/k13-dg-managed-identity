using System;
using System.Collections.Generic;

using CMS.Ecommerce;
using CMS.DocumentEngine;

using DancingGoat.Repositories.Filters;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of products.
    /// </summary>
    public interface IProductRepository : IRepository
    {
        /// <summary>
        /// Returns the product with the specified identifier.
        /// </summary>
        /// <param name="nodeGUID">The product identifier.</param>
        /// <returns>The product with the specified node identifier, if found; otherwise, null.</returns>
        SKUTreeNode GetProduct(Guid nodeGUID);


        /// <summary>
        /// Returns the product with the specified SKU identifier.
        /// </summary>
        /// <param name="skuId">The product or variant SKU identifier.</param>
        /// <returns>The product with the specified SKU identifier, if found; otherwise, null.</returns>
        SKUTreeNode GetProductForSKU(int skuId);
       

        /// <summary>
        /// Returns an enumerable collection of products ordered by the date of publication.
        /// </summary>
        /// <param name="statusId">The products with status identifier.</param>
        /// <param name="count">The number of products to return. Use 0 as value to return all records.</param>
        /// <returns>An enumerable collection that contains the specified number of products.</returns>
        IEnumerable<SKUTreeNode> GetProductsByStatus(int statusId, int count = 0);


        /// <summary>
        /// Returns an enumerable collection of products.
        /// </summary>
        /// <param name="filter">Repository filter.</param>
        /// <param name="classNames">Class names of products to retrieve.</param>
        /// <returns>An enumerable collection that contains the specified products.</returns>
        IEnumerable<SKUTreeNode> GetProducts(IRepositoryFilter filter, params string[] classNames);


        /// <summary>
        /// Returns an enumerable products collection from the given path ordered by <see cref="TreeNode.NodeOrder"/>.
        /// </summary>
        /// <param name="parentPageAliasPath">Parent page alias path.</param>
        /// <param name="count">The number of products to return. Use 0 as value to return all records.</param>
        IEnumerable<SKUTreeNode> GetProducts(string parentPageAliasPath, int count = 0);
    }
}