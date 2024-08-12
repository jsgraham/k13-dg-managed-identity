using System.Collections.Generic;
using System.Web.Mvc;

using CMS.Ecommerce;
using CMS.Tests;

using DancingGoat.Controllers;
using DancingGoat.Repositories;

using Kentico.Content.Web.Mvc;

using NSubstitute;
using NUnit.Framework;

namespace DancingGoat.Tests
{
    /// <summary>
    /// Tests for <see cref="ProductController"/> class.
    /// </summary>
    public class ProductControllerTests
    {
        [TestFixture]
        public class DetailTests : UnitTests
        {
            private const int SKU_ID = 1;
            private const int VARIANT_ID = 11;

            private IPageDataContextRetriever retiever;
            private IVariantRepository variantRepository;
            private ProductController controller;
            private SKUInfo sku;
            private SKUInfo variant;
            private SKUTreeNode page;


            [SetUp]
            public void SetUp()
            {
                Fake<SKUInfo, SKUInfoProvider>().WithData(sku = new SKUInfo
                {
                    SKUID = SKU_ID
                }, variant = new SKUInfo
                {
                    SKUID = VARIANT_ID
                });
                Fake<VariantOptionInfo, VariantOptionInfoProvider>().WithData();

                page = Substitute.For<SKUTreeNode>();

                retiever = Substitute.For<IPageDataContextRetriever>();
                variantRepository = Substitute.For<IVariantRepository>();
                controller = new ProductController(retiever, null, variantRepository, null, null, SKUInfo.Provider);
            }


            [Test]
            public void NoSku_Returns_HttpNotFoundResult()
            {
                page.NodeSKUID.Returns(0);
                var data = Substitute.For<IPageDataContext<SKUTreeNode>>();
                data.Page.Returns(page);

                retiever.Retrieve<SKUTreeNode>().Returns(data);

                var detail = controller.Detail();

                Assert.IsInstanceOf<HttpNotFoundResult>(detail);
            }


            [Test]
            public void DisabledSku_Returns_HttpNotFoundResult()
            {
                sku.SKUEnabled = false;
                page.NodeSKUID.Returns(SKU_ID);
                var data = Substitute.For<IPageDataContext<SKUTreeNode>>();
                data.Page.Returns(page);

                retiever.Retrieve<SKUTreeNode>().Returns(data);

                var detail = controller.Detail();

                Assert.IsInstanceOf<HttpNotFoundResult>(detail);
            }


            [Test]
            public void AllDisabledSkuVariants_Returns_HttpNotFoundResult()
            {
                sku.SKUEnabled = true;
                variant.SKUEnabled = false;
                page.NodeSKUID.Returns(SKU_ID);
                var data = Substitute.For<IPageDataContext<SKUTreeNode>>();
                data.Page.Returns(page);

                var variants = new List<ProductVariant>()
                {
                    new ProductVariant(VARIANT_ID)
                };

                retiever.Retrieve<SKUTreeNode>().Returns(data);
                variantRepository.GetByProductId(SKU_ID).Returns(variants);

                var detail = controller.Detail();

                Assert.IsInstanceOf<HttpNotFoundResult>(detail);
            }
        }
    }
}