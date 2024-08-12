using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Ecommerce;

using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;

using DancingGoat.Controllers;
using DancingGoat.Infrastructure;
using DancingGoat.Models.Manufacturers;
using DancingGoat.Models.Products;
using DancingGoat.Repositories;
using DancingGoat.Services;

[assembly: RegisterPageRoute("DancingGoatMVC.ManufacturerSection", typeof(ManufacturersController))]
[assembly: RegisterPageRoute("DancingGoatMVC.Manufacturer", typeof(ManufacturersController), ActionName = "Detail")]

namespace DancingGoat.Controllers
{
    public class ManufacturersController : Controller
    {
        private readonly IPageDataContextRetriever dataRetriever;
        private readonly IManufacturerRepository manufacturerRepository;
        private readonly IProductRepository productRepository;
        private readonly ICalculationService calculationService;
        private readonly IPublicStatusRepository publicStatusRepository;
        private readonly IOutputCacheDependencies outputCacheDependencies;
        private readonly IPageUrlRetriever pageUrlRetriever;


        public ManufacturersController(IPageDataContextRetriever dataRetriever,
            IManufacturerRepository manufacturerRepository, 
            IProductRepository productRepository, 
            ICalculationService calculationService,
            IPublicStatusRepository publicStatusRepository, 
            IOutputCacheDependencies outputCacheDependencies,
            IPageUrlRetriever pageUrlRetriever)
        {
            this.dataRetriever = dataRetriever;
            this.manufacturerRepository = manufacturerRepository;
            this.productRepository = productRepository;
            this.calculationService = calculationService;
            this.publicStatusRepository = publicStatusRepository;
            this.outputCacheDependencies = outputCacheDependencies;
            this.pageUrlRetriever = pageUrlRetriever;
        }


        [OutputCache(CacheProfile = "Default")]
        public ActionResult Index()
        {
            var manufacturers = manufacturerRepository.GetManufacturers(ContentItemIdentifiers.MANUFACTURERS);
            outputCacheDependencies.AddDependencyOnPages(manufacturers);

            var model = GetManufacturersViewModel(manufacturers);

            return View(model);
        }


        public ActionResult Detail()
        {
            var manufacturer = dataRetriever.Retrieve<Manufacturer>().Page;
            var manufactutersProducts = productRepository.GetProducts(manufacturer.NodeAliasPath);
            var model = GetProductsViewModel(manufactutersProducts, manufacturer.NodeAlias);

            return View(new ManufacturerDetailViewModel(manufacturer, model));
        }


        private IEnumerable<ManufactureListItemViewModel> GetManufacturersViewModel(IEnumerable<Manufacturer> manufacturers)
        {
            return manufacturers.Select(manufacturer => new ManufactureListItemViewModel(manufacturer, pageUrlRetriever));
        }


        private IEnumerable<ProductListItemViewModel> GetProductsViewModel(IEnumerable<SKUTreeNode> products, string pageAlias)
        {
            return products.Select(
                product => new ProductListItemViewModel(
                    product,
                    calculationService.CalculatePrice(product.SKU),
                    publicStatusRepository.GetById(product.SKU.SKUPublicStatusID)?.PublicStatusDisplayName,
                    pageUrlRetriever) 
                {
                    CategoryName = pageAlias 
                });
        }
    }
}