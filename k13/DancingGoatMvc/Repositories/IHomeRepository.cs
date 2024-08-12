using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CMS.DocumentEngine.Types.DancingGoatMvc;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of home page sections.
    /// </summary>
    public interface IHomeRepository : IRepository
    {
        /// <summary>
        /// Asynchronously returns an enumerable collection of home page sections ordered by a position in the content tree.
        /// </summary>
        /// <param name="nodeAliasPath">The node alias path of the Home in the content tree.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        Task<IEnumerable<HomeSection>> GetHomeSectionsAsync(string nodeAliasPath, CancellationToken cancellationToken);


        /// <summary>
        /// Returns an enumerable collection of home page sections ordered by a position in the content tree.
        /// </summary>
        /// <param name="nodeAliasPath">The node alias path of the Home in the content tree.</param>
        IEnumerable<HomeSection> GetHomeSections(string nodeAliasPath);

    }
}