using System;
using System.Linq;

using CMS.DocumentEngine;

using Kentico.Content.Web.Mvc;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of pages.
    /// </summary>
    public class KenticoPageRepository : IPageRepository
    {
        private readonly IPageRetriever pageRetriever;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoPageRepository"/> class that returns page.
        /// </summary>
        /// <param name="pageRetriever">Retriever for pages based on given parameters.</param>
        public KenticoPageRepository(IPageRetriever pageRetriever)
        {
            this.pageRetriever = pageRetriever;
        }


        /// <summary>
        /// Returns a page with the specified identifier.
        /// </summary>
        /// <param name="nodeGuid">The page node identifier.</param>
        public TreeNode Get(Guid nodeGuid)
        {
            return pageRetriever.Retrieve<TreeNode>(
                query => query
                    .WhereEquals("NodeGUID", nodeGuid),
                cache => cache
                    .Key($"{nameof(KenticoPageRepository)}|{nameof(Get)}|{nodeGuid}"))
                .FirstOrDefault();
        }
    }
}