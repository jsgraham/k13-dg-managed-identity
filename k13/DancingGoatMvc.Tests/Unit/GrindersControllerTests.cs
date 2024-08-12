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
using DancingGoat.Models.Grinders;
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
    public class GrindersControllerTests : UnitTests
    {
        private const string GRINDER_TITLE1 = "Grinder1";
        private const string GRINDER_TITLE2 = "Grinder2";
        private const string URL = "testurl";

        private GrindersController controller;
        private GrinderFilterViewModel filter;
        private GrinderFilterViewModel filter2;
        private GrinderFilterViewModel filter3;


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
            controller = new GrindersController(repository, calculationService, pageUrlRetriever);
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
                .ShouldRenderPartialView("GrindersList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models =>
                    models.Any(item => item.Name == GRINDER_TITLE1)
                    && models.Any(y => y.Name == GRINDER_TITLE2)
                );
        }


        [Test]
        public void Filter_ApplyRestrictiveFilter_RendersDefaultViewWithFilteredData()
        {
            MockHttpContext(true);

            controller.WithCallTo(c => c.Filter(filter2))
                .ShouldRenderPartialView("GrindersList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models =>
                    models.Any(item => item.Name == GRINDER_TITLE2)
                    && models.Count() == 1
                );
        }


        [Test]
        public void Filter_ApplyFullRestrictiveFilter_RendersDefaultViewWithoutData()
        {
            MockHttpContext(true);

            controller.WithCallTo(c => c.Filter(filter3))
                .ShouldRenderPartialView("GrindersList")
                .WithModelMatchingCondition<IEnumerable<ProductListItemViewModel>>(models => !models.Any());
        }


        private IProductRepository MockDataSource(SKUInfo skuInfo)
        {
            Fake().DocumentType<ManualGrinder>(ManualGrinder.CLASS_NAME);
            Fake().DocumentType<ElectricGrinder>(ElectricGrinder.CLASS_NAME);

            var grinder1 = TreeNode.New<ManualGrinder>().With(x =>
            {
                x.DocumentName = GRINDER_TITLE1;
                x.SKU = skuInfo;
            });

            var grinder2 = TreeNode.New<ElectricGrinder>().With(x =>
            {
                x.DocumentName = GRINDER_TITLE2;
                x.SKU = skuInfo;
            });

            var repository = Substitute.For<IProductRepository>();
            var grinderClasses = new[] { ManualGrinder.CLASS_NAME, ElectricGrinder.CLASS_NAME };

            filter = Substitute.For<GrinderFilterViewModel>();
            repository.GetProducts(filter, Arg.Is<string[]>(collection => collection.SequenceEqual(grinderClasses)))
                .Returns(new List<SKUTreeNode> { grinder1, grinder2 });

            filter2 = Substitute.For<GrinderFilterViewModel>();
            repository.GetProducts(filter2, Arg.Is<string[]>(collection => collection.SequenceEqual(grinderClasses)))
                .Returns(new List<SKUTreeNode> { grinder2 });

            filter3 = Substitute.For<GrinderFilterViewModel>();
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
