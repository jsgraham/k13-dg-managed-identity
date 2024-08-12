using System.Collections.Generic;

using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Content.Web.Mvc;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a contract for manufacturers.
    /// </summary>
    public class KenticoManufacturerRepository : IManufacturerRepository
    {
        private readonly IPageRetriever pageRetriever;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoManufacturerRepository"/> class that returns manufacturers.
        /// </summary>
        /// <param name="pageRetriever">Retriever for pages based on given parameters.</param>
        public KenticoManufacturerRepository(IPageRetriever pageRetriever)
        {
            this.pageRetriever = pageRetriever;
        }


        /// <summary>
        /// Returns an enumerable collection of manufacturers.
        /// </summary>
        /// <param name="parentAliasPath">Parent node alias path.</param>
        public IEnumerable<Manufacturer> GetManufacturers(string parentAliasPath)
        {
            return pageRetriever.Retrieve<Manufacturer>(
                query => query
                    .Path(parentAliasPath, PathTypeEnum.Children)
                    .OrderBy("NodeOrder"),
                cache => cache
                    .Key($"{nameof(KenticoManufacturerRepository)}|{nameof(GetManufacturers)}|{parentAliasPath}")
                    // Include path dependency to flush cache when a new child page is created or page order is changed.
                    .Dependencies((_, builder) => builder.PagePath(parentAliasPath, PathTypeEnum.Children).PageOrder()));
        }
    }
}