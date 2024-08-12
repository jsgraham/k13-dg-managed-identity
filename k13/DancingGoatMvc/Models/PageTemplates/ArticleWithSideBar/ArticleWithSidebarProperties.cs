using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

namespace DancingGoat.Models.PageTemplates
{
    public class ArticleWithSideBarProperties : IPageTemplateProperties
    {
        [EditingComponent(DropDownComponent.IDENTIFIER, Label = "{$dancinggoatmvc.pagetemplate.articlewithsidebar.sidebarlocation$}", Order = 1)]
        [EditingComponentProperty(nameof(DropDownProperties.DataSource), "Right;{$dancinggoatmvc.pagetemplate.articlewithsidebar.right$}\r\nLeft;{$dancinggoatmvc.pagetemplate.articlewithsidebar.left$}")]
        public string SidebarLocation { get; set; } = "Right";

        [EditingComponent(DropDownComponent.IDENTIFIER, Label = "{$dancinggoatmvc.pagetemplate.articlewithsidebar.articlewidth$}", Order = 2)]
        [EditingComponentProperty(nameof(DropDownProperties.DataSource), "3;25%\r\n6;50%\r\n9;75%")]
        public string ArticleWidth { get; set; } = "9";
    }
}