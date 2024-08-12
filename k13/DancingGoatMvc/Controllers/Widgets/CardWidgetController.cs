using System;
using System.Linq;
using System.Web.Mvc;

using CMS.MediaLibrary;
using CMS.SiteProvider;

using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Infrastructure;
using DancingGoat.Models.Widgets;
using DancingGoat.Repositories;

[assembly: RegisterWidget(CardWidgetController.IDENTIFIER, typeof(CardWidgetController), "{$dancinggoatmvc.widget.card.name$}", Description = "{$dancinggoatmvc.widget.card.description$}", IconClass = "icon-rectangle-paragraph")]

namespace DancingGoat.Controllers.Widgets
{
    /// <summary>
    /// Controller for card widget.
    /// </summary>
    public class CardWidgetController : WidgetController<CardWidgetProperties>
    {
        /// <summary>
        /// Widget identifier.
        /// </summary>
        public const string IDENTIFIER = "DancingGoat.LandingPage.CardWidget";


        private readonly IMediaFileRepository mediaFileRepository;
        private readonly IOutputCacheDependencies outputCacheDependencies;
        private readonly IComponentPropertiesRetriever componentPropertiesRetriever;
        private readonly IMediaFileUrlRetriever mediaFileUrlRetriever;


        /// <summary>
        /// Creates an instance of <see cref="CardWidgetController"/> class.
        /// </summary>
        public CardWidgetController(IMediaFileRepository mediaFileRepository,
            IOutputCacheDependencies outputCacheDependencies,
            IComponentPropertiesRetriever componentPropertiesRetriever,
            IMediaFileUrlRetriever mediaFileUrlRetriever)
        {
            this.mediaFileRepository = mediaFileRepository;
            this.outputCacheDependencies = outputCacheDependencies;
            this.componentPropertiesRetriever = componentPropertiesRetriever;
            this.mediaFileUrlRetriever = mediaFileUrlRetriever;
        }


        public ActionResult Index()
        {
            var properties = componentPropertiesRetriever.Retrieve<CardWidgetProperties>();
            var image = GetImage(properties);

            outputCacheDependencies.AddDependencyOnInfoObject<MediaFileInfo>(image?.FileGUID ?? Guid.Empty);

            return PartialView("Widgets/_CardWidget", new CardWidgetViewModel
            {
                ImagePath = image == null ? null : mediaFileUrlRetriever.Retrieve(image).RelativePath,
                Text = properties.Text
            });
        }


        private MediaFileInfo GetImage(CardWidgetProperties properties)
        {
            var imageGuid = properties.Image.FirstOrDefault()?.FileGuid ?? Guid.Empty;
            if (imageGuid == Guid.Empty)
            {
                return null;
            }

            return mediaFileRepository.GetMediaFile(imageGuid, SiteContext.CurrentSiteID);
        }
    }
}