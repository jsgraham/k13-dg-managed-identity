using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using DancingGoat.Controllers.PageTemplates;
using DancingGoat.Models.PageTemplates;

using Kentico.Content.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

[assembly: RegisterPageTemplate("DancingGoat.ArticleWithSidebar", typeof(ArticleWithSidebarPageTemplateController), "{$dancinggoatmvc.pagetemplate.articlewithsidebar.name$}", Description = "{$dancinggoatmvc.pagetemplate.articlewithsidebar.description$}", IconClass = "icon-l-text-col")]

namespace DancingGoat.Controllers.PageTemplates
{
    public class ArticleWithSidebarPageTemplateController : PageTemplateController<ArticleWithSideBarProperties>
    {
        private readonly IPageDataContextRetriever pageDataContextRetriver;
        private readonly IPageUrlRetriever pageUrlRetriever;
        private readonly IPageAttachmentUrlRetriever attachmentUrlRetriever;
        private readonly IPageTemplatePropertiesRetriever pageTemplatePropertiesRetriever;


        public ArticleWithSidebarPageTemplateController(
            IPageDataContextRetriever pageDataContextRetriver,
            IPageUrlRetriever pageUrlRetriever,
            IPageAttachmentUrlRetriever attachmentUrlRetriever,
            IPageTemplatePropertiesRetriever pageTemplatePropertiesRetriever)
        {
            this.pageDataContextRetriver = pageDataContextRetriver;
            this.pageUrlRetriever = pageUrlRetriever;
            this.attachmentUrlRetriever = attachmentUrlRetriever;
            this.pageTemplatePropertiesRetriever = pageTemplatePropertiesRetriever;
        }


        public ActionResult Index()
        {
            var article = pageDataContextRetriver.Retrieve<Article>().Page;
            if (article == null)
            {
                return HttpNotFound();
            }

            var templateProperties = pageTemplatePropertiesRetriever.Retrieve<ArticleWithSideBarProperties>();

            return View("PageTemplates/_ArticleWithSidebar", ArticleWithSideBarViewModel.GetViewModel(article, templateProperties, pageUrlRetriever, attachmentUrlRetriever));
        }
    }
}