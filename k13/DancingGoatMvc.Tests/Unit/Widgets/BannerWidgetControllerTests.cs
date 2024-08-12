using System;
using System.Collections.Generic;
using System.Linq;

using CMS.SiteProvider;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Tests;
using CMS.MediaLibrary;
using CMS.DocumentEngine;

using Kentico.PageBuilder.Web.Mvc;
using Kentico.Components.Web.Mvc.FormComponents;
using Kentico.Content.Web.Mvc;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Models.Widgets;
using DancingGoat.Repositories;
using DancingGoat.Infrastructure;

using Tests.DocumentEngine;

using NSubstitute;
using NUnit.Framework;
using TestStack.FluentMVCTesting;

namespace DancingGoat.Tests.Unit
{
    public class BannerWidgetControllerTests : UnitTests
    {
        private const string PARTIAL_VIEW_NAME = "Widgets/_BannerWidget";
        private const int SITE_ID = 1;
        private const string BANNER_TEXT = "Banner text";

        private BannerWidgetController controller;
        private readonly IMediaFileRepository mediaFileRepository = Substitute.For<IMediaFileRepository>();
        private readonly IOutputCacheDependencies outputCacheDependencies = Substitute.For<IOutputCacheDependencies>();
        private readonly IMediaFileUrlRetriever fileUrlRetriever = Substitute.For<IMediaFileUrlRetriever>();
        private readonly IComponentPropertiesRetriever propertiesRetriever = Substitute.For<IComponentPropertiesRetriever>();
        private readonly IPageBuilderDataContextRetriever pageBuilderDataContextRetriever = Substitute.For<IPageBuilderDataContextRetriever>();
        private readonly Guid fileGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");


        [SetUp]
        public void SetUp()
        {
            Fake<MediaFileInfo>();
            Fake().DocumentType<Article>(Article.CLASS_NAME);
            mediaFileRepository.GetMediaFile(fileGuid, Arg.Any<int>()).Returns(new MediaFileInfo
            {
                FileGUID = fileGuid,
                FileSiteID = SITE_ID
            });
            var fileUrl = Substitute.For<IMediaFileUrl>();
            fileUrl.RelativePath.Returns("~/path");
            fileUrlRetriever.Retrieve(Arg.Any<MediaFileInfo>()).Returns(fileUrl);
            Fake<SiteInfo, SiteInfoProvider>().WithData(
                new SiteInfo
                {
                    SiteID = SITE_ID,
                    SiteName = "Site"
                });

            controller = new BannerWidgetController(mediaFileRepository, outputCacheDependencies, fileUrlRetriever, propertiesRetriever);
            controller.ControllerContext = ControllerContextMock.GetControllerContext(controller);
        }


        [Test]
        public void Index_RetrieveExistingFile_ReturnsCorrectModel()
        {
            propertiesRetriever
                .Retrieve<BannerWidgetProperties>()
                .Returns(new BannerWidgetProperties { Image = new List<MediaFilesSelectorItem> { new MediaFilesSelectorItem { FileGuid = fileGuid } }, Text = BANNER_TEXT });

            controller.WithCallTo(c => c.Index())
                .ShouldRenderPartialView(PARTIAL_VIEW_NAME)
                .WithModel<BannerWidgetViewModel>(m =>
                {
                    Assert.Multiple(() =>
                    {
                        Assert.That(m.ImagePath, Is.EqualTo("~/path"));
                        Assert.That(m.Text, Is.EqualTo(BANNER_TEXT));
                    });
                });
        }


        [Test]
        public void Index_RetrieveNotExistingFile_ReturnsModelWithEmptyImage()
        {
            propertiesRetriever
                .Retrieve<BannerWidgetProperties>()
                .Returns(new BannerWidgetProperties { Image = Enumerable.Empty<MediaFilesSelectorItem>(), Text = BANNER_TEXT });

            Assert.Multiple(() =>
            {
                controller.WithCallTo(c => c.Index())
                    .ShouldRenderPartialView(PARTIAL_VIEW_NAME)
                    .WithModel<BannerWidgetViewModel>(m =>
                    {
                        Assert.Multiple(() =>
                        {
                            Assert.That(m.ImagePath, Is.Null);
                            Assert.That(m.Text, Is.EqualTo(BANNER_TEXT));
                        });
                    });
            });
        }
    }
}
