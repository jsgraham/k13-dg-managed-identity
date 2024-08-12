using System.Linq;
using System.Web.Mvc;

using DancingGoat.Infrastructure;
using DancingGoat.Models.Navigation;
using DancingGoat.Repositories;

using Kentico.Content.Web.Mvc;

namespace DancingGoat.Controllers
{
    public class NavigationController : Controller
    {
        private readonly INavigationRepository navigationRepository;
        private readonly IPageUrlRetriever pageUrlRetriever;
        private readonly IOutputCacheDependencies outputCacheDependencies;


        public NavigationController(INavigationRepository navigationRepository, IOutputCacheDependencies outputCacheDependencies, IPageUrlRetriever pageUrlRetriever)
        {
            this.navigationRepository = navigationRepository;
            this.pageUrlRetriever = pageUrlRetriever;
            this.outputCacheDependencies = outputCacheDependencies;
        }


        [ChildActionOnly]
        public PartialViewResult Menu()
        {
            var menuItems = navigationRepository.GetMenuItems();
            var menuItemsModel = menuItems.Select(menuItem => MenuItemModel.GetViewModel(menuItem, pageUrlRetriever));

            outputCacheDependencies.AddDependencyOnPages(menuItems);

            return PartialView("Navigation/_Menu", new MenuViewModel { Items = menuItemsModel });
        }


        [ChildActionOnly]
        public PartialViewResult Footer()
        {
            var footerNavigationItems = navigationRepository.GetFooterNavigationItems();
            var footerNavigationItemsModel = footerNavigationItems.Select(item => MenuItemModel.GetViewModel(item, pageUrlRetriever));

            outputCacheDependencies.AddDependencyOnPages(footerNavigationItems);

            return PartialView("Navigation/_Footer", new MenuViewModel { Items = footerNavigationItemsModel });
        }
    }
}