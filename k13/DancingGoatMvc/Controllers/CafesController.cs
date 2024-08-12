using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Globalization;
using CMS.DocumentEngine;

using Kentico.Content.Web.Mvc.Routing;
using Kentico.Content.Web.Mvc;

using DancingGoat.Controllers;
using DancingGoat.Infrastructure;
using DancingGoat.Models.Cafes;
using DancingGoat.Models.Contacts;
using DancingGoat.Repositories;

[assembly: RegisterPageRoute(CafeSection.CLASS_NAME, typeof(CafesController))]

namespace DancingGoat.Controllers
{
    public class CafesController : Controller
    {
        private readonly IPageDataContextRetriever dataContextRetriever;
        private readonly ICafeRepository cafeRespository;
        private readonly ICountryRepository countryRepository;
        private readonly IOutputCacheDependencies outputCacheDependencies;
        private readonly IPageAttachmentUrlRetriever attachmentUrlRetriever;


        public CafesController(IPageDataContextRetriever dataContextRetriever,
            ICafeRepository cafeRespository,
            ICountryRepository countryRepository,
            IOutputCacheDependencies outputCacheDependencies, 
            IPageAttachmentUrlRetriever attachmentUrlRetriever)
        {
            this.dataContextRetriever = dataContextRetriever;
            this.cafeRespository = cafeRespository;
            this.countryRepository = countryRepository;
            this.outputCacheDependencies = outputCacheDependencies;
            this.attachmentUrlRetriever = attachmentUrlRetriever;
        }


        [OutputCache(CacheProfile = "Default")]
        public ActionResult Index()
        {
            var section = dataContextRetriever.Retrieve<TreeNode>().Page;
            var companyCafes = cafeRespository.GetCompanyCafes(section.NodeAliasPath, 4);
            var partnerCafes = cafeRespository.GetPartnerCafes(section.NodeAliasPath);

            var model = new Models.Cafes.IndexViewModel
            {
                CompanyCafes = GetCompanyCafesModel(companyCafes),
                PartnerCafes = GetPartnerCafesModel(partnerCafes)
            };

            outputCacheDependencies.AddDependencyOnPages(companyCafes);
            outputCacheDependencies.AddDependencyOnPages(partnerCafes);
            outputCacheDependencies.AddDependencyOnInfoObjects<CountryInfo>();
            outputCacheDependencies.AddDependencyOnInfoObjects<StateInfo>();

            return View(model);
        }


        private Dictionary<string, List<ContactViewModel>> GetPartnerCafesModel(IEnumerable<Cafe> cafes)
        {
            var cityCafes = new Dictionary<string, List<ContactViewModel>>();

            // Group partner cafes by their location
            foreach (var cafe in cafes)
            {
                var city = cafe.City.ToLowerInvariant();
                var contact = ContactViewModel.GetViewModel(cafe, countryRepository);

                if (cityCafes.ContainsKey(city))
                {
                    cityCafes[city].Add(contact);
                }
                else
                {
                    cityCafes.Add(city, new List<ContactViewModel> {contact});
                }
            }

            return cityCafes;
        }


        private IEnumerable<CafeViewModel> GetCompanyCafesModel(IEnumerable<Cafe> cafes)
        {
            return cafes.Select(cafe => CafeViewModel.GetViewModel(cafe, countryRepository, attachmentUrlRetriever));
        }
    }
}