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
using DancingGoat.Models.Coffees;
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
    public class CoffeesControllerTests : UnitTests
    {
        private const string COFFEE_TITLE1 = "Coffee1";
        private const string COFFEE_TITLE2 = "Coffee2";
        private const string URL = "testurl";

        private CoffeesController controller;
        private CoffeeFilterViewModel filter;
        private CoffeeFilterViewModel filter2;
        private CoffeeFilterViewModel filter3;


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
            controller = new CoffeesController(repository, calculationService, pageUrlRetriever);
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
                .ShouldRenderPartialView("CoffeeList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models => 
                    models.Any(item => item.Name == COFFEE_TITLE1)
                    && models.Any(y => y.Name == COFFEE_TITLE2)
                );
        }


        [Test]
        public void Filter_ApplyRestrictiveFilter_RendersDefaultViewWithFilteredData()
        {
            MockHttpContext(true);

            controller.WithCallTo(c => c.Filter(filter2))
                .ShouldRenderPartialView("CoffeeList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models => 
                    models.Any(item => item.Name == COFFEE_TITLE2) 
                    && models.Count() == 1
                );
        }


        [Test]
        public void Filter_ApplyFullRestrictiveFilter_RendersDefaultViewWithoutData()
        {
            MockHttpContext(true);

            controller.WithCallTo(c => c.Filter(filter3))
                .ShouldRenderPartialView("CoffeeList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models => !models.Any());
        }


        private ICoffeeRepository MockDataSource(SKUInfo skuInfo)
        {
            Fake().DocumentType<Coffee>(Coffee.CLASS_NAME);

            var coffee1 = TreeNode.New<Coffee>().With(x =>
            {
                x.Fields.IsDecaf = false;
                x.DocumentName = COFFEE_TITLE1;
                x.SKU = skuInfo;
            });

            var coffee2 = TreeNode.New<Coffee>().With(x =>
            {
                x.Fields.IsDecaf = true;
                x.DocumentName = COFFEE_TITLE2;
                x.SKU = skuInfo;
            });

            var repository = Substitute.For<ICoffeeRepository>();

            // Filter without restriction
            filter = Substitute.For<CoffeeFilterViewModel>();
            repository.GetCoffees(filter).Returns(new List<Coffee> { coffee1, coffee2 });

            // Filter for decafed coffees
            filter2 = Substitute.For<CoffeeFilterViewModel>();
            repository.GetCoffees(filter2).Returns(new List<Coffee> { coffee2 });

            // There is no coffee for this filter
            filter3 = Substitute.For<CoffeeFilterViewModel>();
            repository.GetCoffees(filter3).Returns(new List<Coffee>());

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
