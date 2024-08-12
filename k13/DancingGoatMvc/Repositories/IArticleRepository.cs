using System.Collections.Generic;

using CMS.DocumentEngine.Types.DancingGoatMvc;

namespace DancingGoat.Repositories
{
    /// <summary>
    /// Represents a contract for a collection of articles.
    /// </summary>
    public interface IArticleRepository : IRepository
    {
        /// <summary>
        /// Returns an enumerable collection of articles ordered by the date of publication. The most recent articles come first.
        /// </summary>
        /// <param name="nodeAliasPath">The node alias path of the articles section in the content tree.</param>
        /// <param name="count">The number of articles to return. Use 0 as value to return all records.</param>
        /// <param name="categories">The code names of selected categories of the articles.</param>
        IEnumerable<Article> GetArticles(string nodeAliasPath, int count = 0, IEnumerable<string> categories = null);


        /// <summary>
        /// Return the current article based on the current page data context.
        /// </summary>
        Article GetCurrent();
    }
}