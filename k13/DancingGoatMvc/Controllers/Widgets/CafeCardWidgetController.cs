using System.Linq;
using System.Web.Mvc;

using Kentico.PageBuilder.Web.Mvc;
using Kentico.Content.Web.Mvc;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Models.Widgets;
using DancingGoat.Repositories;
using DancingGoat.Infrastructure;

[assembly: RegisterWidget(CafeCardWidgetController.IDENTIFIER, typeof(CafeCardWidgetController), "{$dancinggoatmvc.widget.cafecard.name$}", Description = "{$dancinggoatmvc.widget.cafecard.description$}", IconClass = "icon-cup")]

namespace DancingGoat.Controllers.Widgets
{
    /// <summary>
    /// Controller for cafe card widget.
    /// </summary>
    public class CafeCardWidgetController : WidgetController<CafeCardProperties>
    {
        /// <summary>
        /// Widget identifier.
        /// </summary>
        public const string IDENTIFIER = "DancingGoat.HomePage.CafeCardWidget";


        private readonly ICafeRepository repository;
        private readonly IOutputCacheDependencies outputCacheDependencies;
        private readonly IComponentPropertiesRetriever componentPropertiesRetriever;
        private readonly IPageAttachmentUrlRetriever attachmentUrlRetriever;


        /// <summary>
        /// Initializes an instance of <see cref="CafeCardWidgetController"/> class.
        /// </summary>
        /// <param name="repository">Repository for retrieving cafes.</param>
        /// <param name="outputCacheDependencies">Output cache dependency handling.</param>
        public CafeCardWidgetController(ICafeRepository repository, 
            IOutputCacheDependencies outputCacheDependencies, 
            IComponentPropertiesRetriever componentPropertiesRetriever, 
            IPageAttachmentUrlRetriever attachmentUrlRetriever)
        {
            this.repository = repository;
            this.outputCacheDependencies = outputCacheDependencies;
            this.componentPropertiesRetriever = componentPropertiesRetriever;
            this.attachmentUrlRetriever = attachmentUrlRetriever;
        }


        public ActionResult Index()
        {
            var selectedPage = componentPropertiesRetriever.Retrieve<CafeCardProperties>().SelectedCafes.FirstOrDefault();
            var cafe = (selectedPage != null) ? repository.GetCafeByGuid(selectedPage.NodeGuid) : null;

            outputCacheDependencies.AddDependencyOnPage(cafe);

            return PartialView("Widgets/_CafeCardWidget", CafeCardViewModel.GetViewModel(cafe, attachmentUrlRetriever));
        }
    }
}