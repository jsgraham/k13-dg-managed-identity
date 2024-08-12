using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.DocumentEngine;

using Kentico.Content.Web.Mvc;
using Kentico.Content.Web.Mvc.Routing;

using DancingGoat.Controllers;
using DancingGoat.Models.Manufacturers;
using DancingGoat.Models.Products;
using DancingGoat.Models.Store;
using DancingGoat.Repositories;
using DancingGoat.Services;

[assembly: RegisterPageRoute("DancingGoatMvc.StoreSection", typeof(StoreController))]

namespace DancingGoat.Controllers
{
    public class StoreController : Controller
    {
        private readonly IPageDataContextRetriever dataRetriever;
        private readonly IPageUrlRetriever pageUrlRetriever;
        private readonly ICalculationService calculationService;
        private readonly IProductRepository productRepository;
        private readonly IPublicStatusRepository publicStatusRepository;
        private readonly IManufacturerRepository manufacturerRepository;
        private readonly IHotTipsRepository hotTipsRepository;


        public StoreController(IPageDataContextRetriever dataRetriever,
            ICalculationService calculationService,
            IProductRepository productRepository,
            IPublicStatusRepository publicStatusRepository,
            IManufacturerRepository manufacturerRepository,
            IHotTipsRepository hotTipsRepository,
            IPageUrlRetriever pageUrlRetriever)
        {
            this.dataRetriever = dataRetriever;
            this.pageUrlRetriever = pageUrlRetriever;
            this.calculationService = calculationService;
            this.productRepository = productRepository;
            this.publicStatusRepository = publicStatusRepository;
            this.manufacturerRepository = manufacturerRepository;
            this.hotTipsRepository = hotTipsRepository;
        }


        public ActionResult Index()
        {
            var featuredProducts = GetBestsellers();
            var manufacturers = GetManufacturers();
            var hotTipProducts = GetHotTipProducts();

            var model = new StoreViewModel
            {
                FeaturedProducts = featuredProducts,
                Manufacturers = manufacturers,
                HotTipProducts = hotTipProducts
            };

            return View(model);
        }


        private IEnumerable<ProductListItemViewModel> GetBestsellers()
        {
            const string bestsellerCodename = "DancingGoatMvc.Bestseller";
            var status = publicStatusRepository.GetByName(bestsellerCodename);

            if (status == null)
            {
                return Enumerable.Empty<ProductListItemViewModel>();
            }

            var products = productRepository.GetProductsByStatus(status.PublicStatusID);

            return products.Select(
                product => new ProductListItemViewModel(
                    product,
                    calculationService.CalculatePrice(product.SKU),
                    status.PublicStatusDisplayName,
                    pageUrlRetriever
                )
            );
        }


        private IEnumerable<ManufactureListItemViewModel> GetManufacturers()
        {
            var manufacturers = manufacturerRepository.GetManufacturers(ContentItemIdentifiers.MANUFACTURERS);

            return manufacturers.Select(manufacturer => new ManufactureListItemViewModel(manufacturer, pageUrlRetriever));
        }


        private IEnumerable<ProductListItemViewModel> GetHotTipProducts()
        {
            var hotTips = hotTipsRepository.GetHotTipProducts(dataRetriever.Retrieve<TreeNode>().Page.NodeAliasPath);

            return hotTips.Select(
                product => new ProductListItemViewModel(
                    product,
                    calculationService.CalculatePrice(product.SKU),
                    publicStatusRepository.GetById(product.SKU.SKUPublicStatusID)?.PublicStatusDisplayName,
                    pageUrlRetriever
                )
            );
        }
    }
}