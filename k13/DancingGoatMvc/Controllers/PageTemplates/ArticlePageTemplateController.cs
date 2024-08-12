using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using DancingGoat.Controllers.PageTemplates;
using DancingGoat.Models.PageTemplates;

using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

[assembly: RegisterPageTemplate("DancingGoat.Article", typeof(ArticlePageTemplateController), "{$dancinggoatmvc.pagetemplate.article.name$}", Description = "{$dancinggoatmvc.pagetemplate.article.description$}", IconClass = "icon-l-text")]

namespace DancingGoat.Controllers.PageTemplates
{
    public class ArticlePageTemplateController : PageTemplateController
    {
        private readonly IPageDataContextRetriever pageDataContextRetriver;
        private readonly IPageUrlRetriever pageUrlRetriever;
        private readonly IPageAttachmentUrlRetriever attachmentUrlRetriever;


        public ArticlePageTemplateController(IPageDataContextRetriever pageDataContextRetriver, IPageUrlRetriever pageUrlRetriever, IPageAttachmentUrlRetriever attachmentUrlRetriever)
        {
            this.pageDataContextRetriver = pageDataContextRetriver;
            this.pageUrlRetriever = pageUrlRetriever;
            this.attachmentUrlRetriever = attachmentUrlRetriever;
        }


        public ActionResult Index()
        {
            var article = pageDataContextRetriver.Retrieve<Article>().Page;
            if (article == null)
            {
                return HttpNotFound();
            }

            return View("PageTemplates/_Article", ArticleViewModel.GetViewModel(article, pageUrlRetriever, attachmentUrlRetriever));
        }
    }
}