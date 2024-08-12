using System.Linq;
using System.Web.Mvc;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Infrastructure;
using DancingGoat.Models.Widgets;
using DancingGoat.Repositories;

using Kentico.PageBuilder.Web.Mvc;

[assembly: RegisterWidget(ProductCardWidgetController.IDENTIFIER, typeof(ProductCardWidgetController), "{$dancinggoatmvc.widget.productcard.name$}", Description = "{$dancinggoatmvc.widget.productcard.description$}", IconClass = "icon-box")]

namespace DancingGoat.Controllers.Widgets
{
    /// <summary>
    /// Controller for product card widget.
    /// </summary>
    public class ProductCardWidgetController : WidgetController<ProductCardProperties>
    {
        /// <summary>
        /// Widget identifier.
        /// </summary>
        public const string IDENTIFIER = "DancingGoat.LandingPage.ProductCardWidget";


        private readonly IProductRepository repository;
        private readonly IOutputCacheDependencies outputCacheDependencies;
        private readonly IComponentPropertiesRetriever componentPropertiesRetriever;


        /// <summary>
        /// Creates an instance of <see cref="ProductCardWidgetController"/> class.
        /// </summary>
        /// <param name="repository">Repository for retrieving products.</param>
        /// <param name="outputCacheDependencies">Output cache dependency handling.</param>
        public ProductCardWidgetController(IProductRepository repository,
            IOutputCacheDependencies outputCacheDependencies,
            IComponentPropertiesRetriever componentPropertiesRetriever)
        {
            this.repository = repository;
            this.outputCacheDependencies = outputCacheDependencies;
            this.componentPropertiesRetriever = componentPropertiesRetriever;
        }


        public ActionResult Index()
        {
            var selectedPage = componentPropertiesRetriever.Retrieve<ProductCardProperties>().SelectedProducts.FirstOrDefault();

            var product = (selectedPage != null) ? repository.GetProduct(selectedPage.NodeGuid) : null;

            outputCacheDependencies.AddDependencyOnPage(product);

            return PartialView("Widgets/_ProductCardWidget", ProductCardViewModel.GetViewModel(product));
        }
    }
}