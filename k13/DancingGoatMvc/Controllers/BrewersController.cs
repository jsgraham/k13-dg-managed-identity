using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Kentico.Content.Web.Mvc.Routing;
using Kentico.Content.Web.Mvc;

using DancingGoat;
using DancingGoat.Controllers;
using DancingGoat.Models.Brewers;
using DancingGoat.Models.Products;
using DancingGoat.Repositories;
using DancingGoat.Repositories.Filters;
using DancingGoat.Services;

[assembly: RegisterPageRoute("DancingGoatMvc.ProductSection", typeof(BrewersController), Path = ContentItemIdentifiers.BREWERS)]

namespace DancingGoat.Controllers
{
    public class BrewersController : Controller
    {
        private readonly IBrewerRepository brewerRepository;
        private readonly ICalculationService calculationService;
        private readonly IPageUrlRetriever pageUrlRetriever;


        public BrewersController(IBrewerRepository brewerRepository, ICalculationService calculationService, IPageUrlRetriever pageUrlRetriever)
        {
            this.brewerRepository = brewerRepository;
            this.calculationService = calculationService;
            this.pageUrlRetriever = pageUrlRetriever;
        }


        // GET: Brewers
        public ActionResult Index()
        {
            var item = GetFilteredBrewers(null);
            var filter = new BrewerFilterViewModel(brewerRepository);
            filter.Load();

            var model = new ProductListViewModel
            {
                Filter = filter,
                Items = item
            };

            return View(model);
        }


        // POST: Filter
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Brewers/Filter")]
        public ActionResult Filter(BrewerFilterViewModel filter)
        {
            if (!Request.IsAjaxRequest())
            {
                return HttpNotFound();
            }

            var items = GetFilteredBrewers(filter);

            return PartialView("BrewersList", items);
        }


        private IEnumerable<ProductListItemViewModel> GetFilteredBrewers(IRepositoryFilter filter)
        {
            var brewers = brewerRepository.GetBrewers(filter);

            var items = brewers.Select(
                brewer => new ProductListItemViewModel(
                    brewer,
                    calculationService.CalculatePrice(brewer.SKU),
                    brewer.Product.PublicStatus?.PublicStatusDisplayName,
                    pageUrlRetriever)
                );
            return items;
        }
    }
}