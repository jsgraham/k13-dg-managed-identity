using CMS.Core;
using CMS.DocumentEngine;

using Kentico.Content.Web.Mvc;

namespace DancingGoat.Models.Navigation
{
    public class MenuItemModel
    {
        public string Caption { get; set; }


        public string Url { get; set; }


        public static MenuItemModel GetViewModel(TreeNode menuItem, IPageUrlRetriever pageUrlRetriever)
        {
            return new MenuItemModel
            {
                Caption = menuItem.DocumentName,
                Url = pageUrlRetriever.Retrieve(menuItem).RelativePath
            };
        }
    }
}