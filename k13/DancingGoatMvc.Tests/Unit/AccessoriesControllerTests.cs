using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Ecommerce;
using CMS.Tests;

using DancingGoat.Controllers;
using DancingGoat.Models.Accessories;
using DancingGoat.Models.Products;
using DancingGoat.Repositories;
using DancingGoat.Services;
using DancingGoat.Tests.Extensions;

using Kentico.Content.Web.Mvc;

using NSubstitute;
using NUnit.Framework;

using Tests.DocumentEngine;
using TestStack.FluentMVCTesting;

namespace DancingGoat.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class AccessoriesControllerTests : UnitTests
    {
        private const string ACCESSORY_TITLE1 = "Accessory1";
        private const string ACCESSORY_TITLE2 = "Accessory2";
        private const string URL = "testurl";

        private AccessoriesController controller;
        private AccessoryFilterViewModel filter;
        private AccessoryFilterViewModel filter2;
        private AccessoryFilterViewModel filter3;


        [SetUp]
        public void SetUp()
        {
            var price = new ProductCatalogPrices(0m, 0m, 0m, 0m, Substitute.For<CurrencyInfo>(), null);
            var calculationService = Substitute.For<ICalculationService>();
            var pageUrlRetriever = Substitute.For<IPageUrlRetriever>();
            pageUrlRetriever.Retrieve(Arg.Any<TreeNode>(), Arg.Any<bool>()).Returns(new PageUrl { RelativePath = URL });

            var skuInfo = Substitute.For<SKUInfo>();

            calculationService.CalculatePrice(skuInfo).Returns(price);

            var repository = MockDataSource(skuInfo);
            controller = new AccessoriesController(repository, calculationService, pageUrlRetriever);
        }


        [Test]
        public void Index_RendersDefaultView()
        {
            controller.WithCallTo(c => c.Index())
                .ShouldRenderDefaultView();
        }


        [Test]
        public void Filter_NonAjaxCall_ReturnsHttpNotFoundResult()
        {
            MockHttpContext(false);

            controller.WithCallTo(c => c.Filter(filter))
                .ShouldGiveHttpStatus(HttpStatusCode.NotFound);

        }


        [Test]
        public void Filter_ApplyNonRestrictiveFilter_RendersDefaultViewWithAllData()
        {
            MockHttpContext(true);

            controller.WithCallTo(c => c.Filter(filter))
                .ShouldRenderPartialView("AccessoriesList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models =>
                    models.Any(item => item.Name == ACCESSORY_TITLE1)
                    && models.Any(y => y.Name == ACCESSORY_TITLE2)
                );
        }


        [Test]
        public void Filter_ApplyRestrictiveFilter_RendersDefaultViewWithFilteredData()
        {
            MockHttpContext(true);

            controller.WithCallTo(c => c.Filter(filter2))
                .ShouldRenderPartialView("AccessoriesList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models =>
                    models.Any(item => item.Name == ACCESSORY_TITLE2)
                    && models.Count() == 1
                    && models.All(item => item.Url == "testurl")
                );
        }


        [Test]
        public void Filter_ApplyFullRestrictiveFilter_RendersDefaultViewWithoutData()
        {
            MockHttpContext(true);

            controller.WithCallTo(c => c.Filter(filter3))
                .ShouldRenderPartialView("AccessoriesList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models => !models.Any());
        }


        private IProductRepository MockDataSource(SKUInfo skuInfo)
        {
            Fake().DocumentType<Tableware>(Tableware.CLASS_NAME);
            Fake().DocumentType<FilterPack>(FilterPack.CLASS_NAME);

            var accessory1 = TreeNode.New<Tableware>().With(x =>
            {
                x.DocumentName = ACCESSORY_TITLE1;
                x.SKU = skuInfo;
            });

            var accessory2 = TreeNode.New<FilterPack>().With(x =>
            {
                x.DocumentName = ACCESSORY_TITLE2;
                x.SKU = skuInfo;
            });

            var repository = Substitute.For<IProductRepository>();
            var grinderClasses = new[] { FilterPack.CLASS_NAME, Tableware.CLASS_NAME };

            filter = Substitute.For<AccessoryFilterViewModel>();
            repository.GetProducts(filter, Arg.Is<string[]>(collection => collection.SequenceEqual(grinderClasses)))
                .Returns(new List<SKUTreeNode> { accessory1, accessory2 });

            filter2 = Substitute.For<AccessoryFilterViewModel>();
            repository.GetProducts(filter2, Arg.Is<string[]>(collection => collection.SequenceEqual(grinderClasses)))
                .Returns(new List<SKUTreeNode> { accessory2 });

            filter3 = Substitute.For<AccessoryFilterViewModel>();
            repository.GetProducts(filter3, Arg.Is<string[]>(collection => collection.SequenceEqual(grinderClasses)))
                .Returns(new List<SKUTreeNode>());

            return repository;
        }


        private void MockHttpContext(bool isAjaxPostback)
        {
            var httpContextSub = Substitute.For<HttpContextBase>();
            var requestSub = Substitute.For<HttpRequestBase>();
            httpContextSub.Request.Returns(requestSub);

            if (isAjaxPostback)
            {
                requestSub.Headers.Returns(new System.Collections.Specialized.NameValueCollection { { "X-Requested-With", "XMLHttpRequest" } });
            }

            controller.ControllerContext = new ControllerContext(httpContextSub, new RouteData(), controller);
        }
    }
}
