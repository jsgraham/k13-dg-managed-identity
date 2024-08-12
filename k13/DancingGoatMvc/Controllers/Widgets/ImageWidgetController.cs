using System;
using System.Linq;
using System.Web.Mvc;

using CMS.DocumentEngine;

using Kentico.PageBuilder.Web.Mvc;
using Kentico.Content.Web.Mvc;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Models.Widgets;
using DancingGoat.Infrastructure;

[assembly: RegisterWidget(ImageWidgetController.IDENTIFIER, typeof(ImageWidgetController), "{$dancinggoatmvc.widget.image.name$}", Description = "{$dancinggoatmvc.widget.image.description$}", IconClass = "icon-picture")]

namespace DancingGoat.Controllers.Widgets
{
    /// <summary>
    /// Controller for image widget.
    /// </summary>
    public class ImageWidgetController : WidgetController<ImageWidgetProperties>
    {
        /// <summary>
        /// Widget identifier.
        /// </summary>
        public const string IDENTIFIER = "DancingGoat.HomePage.ImageWidget";


        private readonly IPageDataContextRetriever pageDataContextRetriever;
        private readonly IOutputCacheDependencies outputCacheDependencies;
        private readonly IComponentPropertiesRetriever propertiesRetriever;
        private readonly IPageAttachmentUrlRetriever attachmentUrlRetriever;


        /// <summary>
        /// Creates an instance of <see cref="ImageWidgetController"/> class.
        /// </summary>
        /// <param name="pageDataContextRetriever">Retriever for page data context.</param>
        /// <param name="outputCacheDependencies">Output cache dependencies.</param>
        /// <param name="propertiesRetriever">Retriever for widget properties.</param>
        public ImageWidgetController(
            IPageDataContextRetriever pageDataContextRetriever,
            IOutputCacheDependencies outputCacheDependencies,
            IComponentPropertiesRetriever propertiesRetriever, 
            IPageAttachmentUrlRetriever attachmentUrlRetriever)
        {
            this.pageDataContextRetriever = pageDataContextRetriever;
            this.outputCacheDependencies = outputCacheDependencies;
            this.propertiesRetriever = propertiesRetriever;
            this.attachmentUrlRetriever = attachmentUrlRetriever;
        }


        public ActionResult Index()
        {
            var properties = propertiesRetriever.Retrieve<ImageWidgetProperties>();
            var image = GetImage(properties);

            outputCacheDependencies.AddDependencyOnPageAttachmnent(image?.AttachmentGUID ?? Guid.Empty);

            return PartialView("Widgets/_ImageWidget", new ImageWidgetViewModel
            {
                ImagePath = image == null ? null : attachmentUrlRetriever.Retrieve(image).RelativePath
            });
        }


        private DocumentAttachment GetImage(ImageWidgetProperties properties)
        {
            var page = pageDataContextRetriever.Retrieve<TreeNode>().Page;
            return page?.AllAttachments.FirstOrDefault(x => x.AttachmentGUID == properties.ImageGuid);
            
        }
    } 
}