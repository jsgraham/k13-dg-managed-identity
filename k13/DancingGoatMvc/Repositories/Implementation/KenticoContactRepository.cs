using System.Linq;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Content.Web.Mvc;

namespace DancingGoat.Repositories.Implementation
{
    /// <summary>
    /// Represents a collection of contact information.
    /// </summary>
    public class KenticoContactRepository : IContactRepository
    {
        private readonly IPageRetriever pageRetriever;


        /// <summary>
        /// Initializes a new instance of the <see cref="KenticoContactRepository"/> class that returns contact information.
        /// </summary>
        /// <param name="pageRetriever">Retriever for pages based on given parameters.</param>
        public KenticoContactRepository(IPageRetriever pageRetriever)
        {
            this.pageRetriever = pageRetriever;
        }


        /// <summary>
        /// Returns company's contact information.
        /// </summary>
        public Contact GetCompanyContact()
        {
            return pageRetriever.Retrieve<Contact>(
                query => query
                    .TopN(1),
                cache => cache
                    .Key($"{nameof(KenticoContactRepository)}|{nameof(GetCompanyContact)}"))
                .FirstOrDefault();
        }
    }
}