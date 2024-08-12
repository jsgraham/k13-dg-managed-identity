using System;

using CMS.Core;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.SiteProvider;
using CMS.Tests;

using Kentico.PageBuilder.Web.Mvc;
using Kentico.Content.Web.Mvc;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Models.Widgets;
using DancingGoat.Infrastructure;

using Tests.DocumentEngine;

using NSubstitute;
using NUnit.Framework;
using TestStack.FluentMVCTesting;


namespace DancingGoat.Tests.Unit
{
    public class ImageWidgetControllerTests : UnitTests
    {
        private const string PARTIAL_VIEW_NAME = "Widgets/_ImageWidget";
        private const int SITE_ID = 1;

        private Article page;
        private ImageWidgetController controller;
        private IPageAttachmentUrlRetriever attachmentUrlRetriever;
        private readonly IComponentPropertiesRetriever propertiesRetriever = Substitute.For<IComponentPropertiesRetriever>();
        private readonly IPageDataContextRetriever pageDataContextRetriever = Substitute.For<IPageDataContextRetriever>();
        private readonly IOutputCacheDependencies outputCacheDependencies = Substitute.For<IOutputCacheDependencies>();        
        private readonly Guid attachmentGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");        


        [SetUp]
        public void SetUp()
        {
            Fake().DocumentType<Article>(Article.CLASS_NAME);
            page = new Article
            {
                DocumentName = "Name"
            };
            var dataContext = Substitute.For<IPageDataContext<TreeNode>>();
            dataContext.Page.Returns(page);
            pageDataContextRetriever.Retrieve<TreeNode>().Returns(dataContext);

            Fake<SiteInfo, SiteInfoProvider>().WithData(
                new SiteInfo
                {
                    SiteID = SITE_ID,
                    SiteName = "Site",
                    SitePresentationURL = "https://presentation.com"
                });
            Fake<AttachmentInfo, AttachmentInfoProvider>().WithData(
                new AttachmentInfo
                {
                    AttachmentGUID = attachmentGuid,
                    AttachmentDocumentID = page.DocumentID,
                    AttachmentSiteID = SITE_ID
                });

            attachmentUrlRetriever = Service.Resolve<IPageAttachmentUrlRetriever>();
            controller = new ImageWidgetController(pageDataContextRetriever, outputCacheDependencies, propertiesRetriever, attachmentUrlRetriever);
            controller.ControllerContext = ControllerContextMock.GetControllerContext(controller);
        }


        [Test]
        public void Index_RetrieveExistingDocumentAttachment_ReturnsCorrectModel()
        {
            propertiesRetriever
                .Retrieve<ImageWidgetProperties>()
                .Returns(new ImageWidgetProperties { ImageGuid = attachmentGuid });

            controller.WithCallTo(c => c.Index())
                .ShouldRenderPartialView(PARTIAL_VIEW_NAME)
                .WithModel<ImageWidgetViewModel>(m => 
                {
                    Assert.That(m.ImagePath, Is.EqualTo("~/getattachment/00000000-0000-0000-0000-000000000001/attachment"));
                });
        }


        [Test]
        public void Index_RetrieveNotExistingDocumentAttachment_ReturnsModelWithEmptyImage()
        {
            propertiesRetriever
                .Retrieve<ImageWidgetProperties>()
                .Returns(new ImageWidgetProperties { ImageGuid = Guid.Parse("00000000-0000-0000-0000-000000000002") });

            controller.WithCallTo(c => c.Index())
                      .ShouldRenderPartialView(PARTIAL_VIEW_NAME)
                      .WithModel<ImageWidgetViewModel>(m => m.ImagePath == null);
        }
    }
}
