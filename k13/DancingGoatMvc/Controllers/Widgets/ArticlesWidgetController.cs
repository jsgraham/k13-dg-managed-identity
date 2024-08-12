using System.Linq;
using System.Web.Mvc;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Infrastructure;
using DancingGoat.Models.Articles;
using DancingGoat.Models.Widgets;
using DancingGoat.Repositories;

using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

[assembly: RegisterWidget(ArticlesWidgetController.IDENTIFIER, typeof(ArticlesWidgetController), "{$dancinggoatmvc.widget.articles.name$}", Description = "{$dancinggoatmvc.widget.articles.description$}", IconClass = "icon-l-list-article")]

namespace DancingGoat.Controllers.Widgets
{
    /// <summary>
    /// Controller for article widget.
    /// </summary>
    public class ArticlesWidgetController : WidgetController<ArticlesWidgetProperties>
    {
        /// <summary>
        /// Widget identifier.
        /// </summary>
        public const string IDENTIFIER = "DancingGoat.HomePage.ArticlesWidget";


        private readonly IArticleRepository repository;
        private readonly IOutputCacheDependencies outputCacheDependencies;
        private readonly IComponentPropertiesRetriever propertiesRetriever;
        private readonly IPageAttachmentUrlRetriever attachmentUrlRetriever;
        private readonly IPageUrlRetriever pageUrlRetriever;


        /// <summary>
        /// Initializes an instance of <see cref="ArticlesWidgetController"/> class.
        /// </summary>
        /// <param name="repository">Articles repository.</param>
        /// <param name="outputCacheDependencies">Output cache dependency handling.</param>
        /// <param name="propertiesRetriever">Retriever for widget properties.</param>
        /// <param name="pageBuilderDataContextRetriever">Retriever for page builder data context.</param>
        /// <param name="pageUrlRetriever">Retriever for page URLs.</param>
        /// <param name="attachmentUrlRetriever">Retriever for page attachment URLs.</param>
        /// <remarks>Use this constructor for tests to handle dependencies.</remarks>
        public ArticlesWidgetController(IArticleRepository repository,
            IOutputCacheDependencies outputCacheDependencies,
            IComponentPropertiesRetriever propertiesRetriever,
            IPageUrlRetriever pageUrlRetriever,
            IPageAttachmentUrlRetriever attachmentUrlRetriever)
        {
            this.repository = repository;
            this.outputCacheDependencies = outputCacheDependencies;
            this.propertiesRetriever = propertiesRetriever;
            this.pageUrlRetriever = pageUrlRetriever;
            this.attachmentUrlRetriever = attachmentUrlRetriever;
        }


        public ActionResult Index()
        {
            var properties = propertiesRetriever.Retrieve<ArticlesWidgetProperties>();
            var articleCategories = properties.Categories?.Select(c => c.Identifier);
            var articles = repository.GetArticles(ContentItemIdentifiers.ARTICLES, properties.Count, articleCategories);
            var articlesModel = articles.Select(article => ArticleViewModel.GetViewModel(article, pageUrlRetriever, attachmentUrlRetriever));

            outputCacheDependencies.AddDependencyOnPages(articles);

            return PartialView("Widgets/_ArticlesWidget", new ArticlesWidgetViewModel { Articles = articlesModel, Count = properties.Count });
        }
    }
}