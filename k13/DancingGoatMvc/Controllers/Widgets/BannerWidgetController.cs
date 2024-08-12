using System;
using System.Linq;
using System.Web.Mvc;

using CMS.MediaLibrary;
using CMS.SiteProvider;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Infrastructure;
using DancingGoat.Models.Widgets;
using DancingGoat.Repositories;

using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

[assembly: RegisterWidget(BannerWidgetController.IDENTIFIER, typeof(BannerWidgetController), "{$dancinggoatmvc.widget.banner.name$}", Description = "{$dancinggoatmvc.widget.banner.description$}", IconClass = "icon-ribbon")]

namespace DancingGoat.Controllers.Widgets
{
    /// <summary>
    /// Controller for banner widget.
    /// </summary>
    public class BannerWidgetController : WidgetController<BannerWidgetProperties>
    {
        /// <summary>
        /// Widget identifier.
        /// </summary>
        public const string IDENTIFIER = "DancingGoat.HomePage.BannerWidget";


        private readonly IMediaFileRepository mediaFileRepository;
        private readonly IOutputCacheDependencies outputCacheDependencies;
        private readonly IMediaFileUrlRetriever mediaFileUrlRetriever;
        private readonly IComponentPropertiesRetriever componentPropertiesRetriever;


        /// <summary>
        /// Creates an instance of <see cref="BannerWidgetController"/> class.
        /// </summary>
        /// <param name="mediaFileRepository">Repository for media files.</param>
        public BannerWidgetController(
            IMediaFileRepository mediaFileRepository,
            IOutputCacheDependencies outputCacheDependencies,
            IMediaFileUrlRetriever mediaFileUrlRetriever,
            IComponentPropertiesRetriever componentPropertiesRetriever)
        {
            this.mediaFileRepository = mediaFileRepository;
            this.outputCacheDependencies = outputCacheDependencies;
            this.mediaFileUrlRetriever = mediaFileUrlRetriever;
            this.componentPropertiesRetriever = componentPropertiesRetriever;
        }


        public ActionResult Index()
        {
            var properties = componentPropertiesRetriever.Retrieve<BannerWidgetProperties>();
            var imagePath = GetImagePath(properties);

            return PartialView("Widgets/_BannerWidget", new BannerWidgetViewModel
            {
                ImagePath = imagePath,
                Text = properties.Text,
                LinkUrl = properties.LinkUrl,
                LinkTitle = properties.LinkTitle
            });
        }


        private string GetImagePath(BannerWidgetProperties properties)
        {
            var imageGuid = properties.Image.FirstOrDefault()?.FileGuid ?? Guid.Empty;
            if (imageGuid == Guid.Empty)
            {
                return null;
            }

            outputCacheDependencies.AddDependencyOnInfoObject<MediaFileInfo>(imageGuid);

            var image = mediaFileRepository.GetMediaFile(imageGuid, SiteContext.CurrentSiteID);
            if (image == null)
            {
                return string.Empty;
            }

            return mediaFileUrlRetriever.Retrieve(image).RelativePath;
        }
    }
}