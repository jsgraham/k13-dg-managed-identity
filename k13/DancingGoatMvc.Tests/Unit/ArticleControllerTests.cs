using System;

using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Tests;

using DancingGoat.Controllers;
using DancingGoat.Infrastructure;
using DancingGoat.Repositories;

using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

using NSubstitute;
using NUnit.Framework;

using Tests.DocumentEngine;
using TestStack.FluentMVCTesting;

namespace DancingGoat.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class ArticlesControllerTests : ContainerNotBuiltUnitTests
    {
        private IPageDataContextRetriever dataContextRetriever;
        private IArticleRepository articleRepository;
        private IPageUrlRetriever pageUrlRetriever;
        private IPageAttachmentUrlRetriever attachmentUrlRetriever;
        private Article article;
        private IOutputCacheDependencies dependencies;
        private const string ARTICLE_TITLE = "Article1";
        private const int DOCUMENT_ID = 1;


        [SetUp]
        public void SetUp()
        {
            var siteService = Substitute.For<ISiteService>();
            var site = Substitute.For<ISiteInfo>();
            site.SiteName = "site";
            siteService.CurrentSite.Returns(site);
            Service.Use<ISiteService>(siteService);

            EnsureServiceContainer();

            Fake().DocumentType<Article>(Article.CLASS_NAME);
            article = TreeNode.New<Article>()
                                .With(a => a.Fields.Title = ARTICLE_TITLE)
                                .With(a => a.SetValue("DocumentID", DOCUMENT_ID));
            dependencies = Substitute.For<IOutputCacheDependencies>();

            dataContextRetriever = Substitute.For<IPageDataContextRetriever>();
            var section = Substitute.For<TreeNode>();
            section.NodeAliasPath.Returns("/section");
            var dataContext = Substitute.For<IPageDataContext<TreeNode>>();
            dataContext.Page.Returns(section);
            dataContextRetriever.Retrieve<TreeNode>().Returns(dataContext);
            articleRepository = Substitute.For<IArticleRepository>();
            pageUrlRetriever = Substitute.For<IPageUrlRetriever>();
            attachmentUrlRetriever = Substitute.For<IPageAttachmentUrlRetriever>();
        }


        [Test]
        public void Index_RendersDefaultView()
        {
            var controller = new ArticlesController(dataContextRetriever, articleRepository, pageUrlRetriever, attachmentUrlRetriever, dependencies);
            controller.WithCallTo(c => c.Index())
                .ShouldRenderDefaultView();
        }


        [Test]
        public void Show_WithExistingArticle_RendersDefaultViewWithCorrectModel()
        {
            articleRepository.GetCurrent().Returns(article);
            var controller = new ArticlesController(dataContextRetriever, articleRepository, pageUrlRetriever, attachmentUrlRetriever, dependencies);
            controller.WithCallTo(c => c.Show())
                .ValidateActionReturnType<TemplateResult>();
        }
    }
}