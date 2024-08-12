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
using DancingGoat.Models.Brewers;
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
    public class BrewersControllerTests : UnitTests
    {
        private const string BREWER_TITLE1 = "Brewer1";
        private const string BREWER_TITLE2 = "Brewer2";
        private const string URL = "testurl";

        private BrewersController controller;
        private BrewerFilterViewModel filter;
        private BrewerFilterViewModel filter2;
        private BrewerFilterViewModel filter3;


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
            controller = new BrewersController(repository, calculationService, pageUrlRetriever);
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
                .ShouldRenderPartialView("BrewersList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models =>
                    models.Any(item => item.Name == BREWER_TITLE1)
                    && models.Any(y => y.Name == BREWER_TITLE2)
                );
        }


        [Test]
        public void Filter_ApplyRestrictiveFilter_RendersDefaultViewWithFilteredData()
        {
            MockHttpContext(true);

            controller.WithCallTo(c => c.Filter(filter2))
                .ShouldRenderPartialView("BrewersList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models =>
                    models.Any(item => item.Name == BREWER_TITLE2)
                    && models.Count() == 1
                );
        }


        [Test]
        public void Filter_ApplyFullRestrictiveFilter_RendersDefaultViewWithoutData()
        {
            MockHttpContext(true);

            controller.WithCallTo(c => c.Filter(filter3))
                .ShouldRenderPartialView("BrewersList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models => !models.Any());
        }


        private IBrewerRepository MockDataSource(SKUInfo skuInfo)
        {
            Fake().DocumentType<Brewer>(Brewer.CLASS_NAME);

            var brewer1 = TreeNode.New<Brewer>().With(x =>
            {
                x.DocumentName = BREWER_TITLE1;
                x.SKU = skuInfo;
            });

            var brewer2 = TreeNode.New<Brewer>().With(x =>
            {
                x.DocumentName = BREWER_TITLE2;
                x.SKU = skuInfo;
            });

            var repository = Substitute.For<IBrewerRepository>();

            filter = Substitute.For<BrewerFilterViewModel>();
            repository.GetBrewers(filter).Returns(new List<Brewer> { brewer1, brewer2 });

            filter2 = Substitute.For<BrewerFilterViewModel>();
            repository.GetBrewers(filter2).Returns(new List<Brewer> { brewer2 });

            filter3 = Substitute.For<BrewerFilterViewModel>();
            repository.GetBrewers(filter3).Returns(new List<Brewer>());

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
