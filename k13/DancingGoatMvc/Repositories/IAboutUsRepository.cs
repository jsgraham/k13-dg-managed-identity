using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CMS.DocumentEngine.Types.DancingGoatMvc;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of stories about company's strategy, history and philosophy.
    /// </summary>
    public interface IAboutUsRepository : IRepository
    {
        /// <summary>
        /// Asynchronously returns an enumerable collection of stories about company's philosophy ordered by a position in the content tree.
        /// </summary>
        /// <param name="nodeAliasPath">The node alias path of the About us section in the content tree.</param>
        /// <param name="cancellationToken">The cancellation instruction.</param>
        Task<IEnumerable<AboutUsSection>> GetSideStoriesAsync(string nodeAliasPath, CancellationToken cancellationToken);



        /// <summary>
        /// Returns an enumerable collection of stories about company's philosophy ordered by a position in the content tree.
        /// </summary>
        /// <param name="nodeAliasPath">The node alias path of the About us section in the content tree.</param>
        IEnumerable<AboutUsSection> GetSideStories(string nodeAliasPath);

    }
}