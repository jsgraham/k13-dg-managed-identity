using System.Collections.Generic;
using System.Linq;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.DocumentEngine;
using CMS.Tests;

using Kentico.PageBuilder.Web.Mvc;
using Kentico.Content.Web.Mvc;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Models.Widgets;
using DancingGoat.Infrastructure;
using DancingGoat.Repositories;

using Kentico.Components.Web.Mvc.FormComponents;

using NSubstitute;
using NUnit.Framework;

using Tests.DocumentEngine;

using TestStack.FluentMVCTesting;

namespace DancingGoat.Tests.Unit
{
    [TestFixture]
    public class ArticlesWidgetControllerTests : UnitTests
    {
        private const string PARTIAL_VIEW_NAME = "Widgets/_ArticlesWidget";
        private const int CURRENT_ARTICLES_COUNT = 2;
        private const int REQUIRED_ARTICLES_COUNT = 5;
        private const string URL = "testurl";

        private readonly IComponentPropertiesRetriever propertiesRetriever = Substitute.For<IComponentPropertiesRetriever>();
        private readonly IArticleRepository articleRepository = Substitute.For<IArticleRepository>();
        private readonly IOutputCacheDependencies outputCacheDependencies = Substitute.For<IOutputCacheDependencies>();
        private readonly IPageUrlRetriever pageUrlRetriever = Substitute.For<IPageUrlRetriever>();
        private readonly IPageAttachmentUrlRetriever attachmentUrlRetriever = Substitute.For<IPageAttachmentUrlRetriever>();


        [Test]
        public void Index_ReturnsCorrectModel()
        {
            Fake().DocumentType<Article>(Article.CLASS_NAME);
            var articlesList = new List<Article>
                {
                    new Article
                    {
                        ArticleTitle = "Title 1",
                    },
                    new Article
                    {
                        ArticleTitle = "Title 2",
                    }
                };

            pageUrlRetriever.Retrieve(Arg.Any<TreeNode>(), Arg.Any<bool>()).Returns(new PageUrl { RelativePath = URL });
            propertiesRetriever.Retrieve<ArticlesWidgetProperties>().Returns(new ArticlesWidgetProperties { Count = REQUIRED_ARTICLES_COUNT });
            articleRepository.GetArticles(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<IEnumerable<string>>()).Returns(articlesList);

            var controller = new ArticlesWidgetController(articleRepository, outputCacheDependencies, propertiesRetriever, pageUrlRetriever, attachmentUrlRetriever);
            controller.ControllerContext = ControllerContextMock.GetControllerContext(controller);

            controller.WithCallTo(c => c.Index())
                .ShouldRenderPartialView(PARTIAL_VIEW_NAME)
                .WithModel<ArticlesWidgetViewModel>(m => m.Articles.Count() == CURRENT_ARTICLES_COUNT && m.Count == REQUIRED_ARTICLES_COUNT);
        }
    }
}
