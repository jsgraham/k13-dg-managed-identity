using System.Collections.Generic;
using System.Linq;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Ecommerce;
using CMS.Tests;

using DancingGoat.Models.Grinders;
using DancingGoat.Repositories;

using NSubstitute;
using NUnit.Framework;

using Tests.DocumentEngine;

namespace DancingGoat.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class GrinderFilterTests : UnitTests
    {
        private IProductRepository repository;

        [SetUp]
        public void SetUp()
        {
            Fake().DocumentType<ManualGrinder>(ManualGrinder.CLASS_NAME);
            Fake().DocumentType<ElectricGrinder>(ElectricGrinder.CLASS_NAME);
            FakeManufacturers();            

            var grinders = MockGrinders();
            var grinderClasses = new[] { ManualGrinder.CLASS_NAME, ElectricGrinder.CLASS_NAME };

            repository = Substitute.For<IProductRepository>();
            repository.GetProducts(null, Arg.Is<string[]>(collection => collection.SequenceEqual(grinderClasses)))
                .Returns(grinders);
        }


        [Test]
        public void Load_FilterContainsCorrectManufacturers()
        {
            var filter = new GrinderFilterViewModel(repository);
            filter.Load();

            var manufacturer1 = filter.Manufacturers.FirstOrDefault(checkbox => checkbox.Value == 1);
            var manufacturer2 = filter.Manufacturers.FirstOrDefault(checkbox => checkbox.Value == 2);

            Assert.Multiple(() => 
                {
                    Assert.AreEqual(2, filter.Manufacturers.Count);
                    Assert.IsNotNull(manufacturer1);
                    Assert.IsNotNull(manufacturer2);
                }
            );
        }


        [Test]
        public void Load_FilterContainsCorrectPrices()
        {
            var filter = new GrinderFilterViewModel(repository);
            filter.Load();

            var toFifty = filter.Prices.FirstOrDefault(checkbox =>
                (int)GrinderPriceRangeEnum.ToFifty == checkbox.Value);

            var fromTwoHundredFiftyToFiveThousand = filter.Prices.FirstOrDefault(checkbox =>
                (int)GrinderPriceRangeEnum.FromFiveHundredToFiveThousand == checkbox.Value);

            var fromFiftyToTwoHundredFifty = filter.Prices.FirstOrDefault(checkbox =>
                (int)GrinderPriceRangeEnum.FromFiftyToFiveHundred == checkbox.Value);

            Assert.Multiple(() =>
                {
                    Assert.AreEqual(3, filter.Prices.Count);
                    Assert.IsNotNull(toFifty);
                    Assert.IsNotNull(fromTwoHundredFiftyToFiveThousand);
                    Assert.IsNotNull(fromFiftyToTwoHundredFifty);
                }
            );
        }


        [Test]
        public void Load_FilterContainsCorrectTypes()
        {
            var filter = new GrinderFilterViewModel(repository);
            filter.Load();

            var status1 = filter.Type.FirstOrDefault(checkbox => checkbox.Value == 0);
            var status2 = filter.Type.FirstOrDefault(checkbox => checkbox.Value == 1);

            Assert.Multiple(() => 
                {
                    Assert.AreEqual(2, filter.Type.Count);
                    Assert.IsNotNull(status1);
                    Assert.IsNotNull(status2);
                }
            );
        }


        [Test]
        public void GetWhereCondition_EmptyViewModel_EmptyWhereCondition()
        {
            var filter = new GrinderFilterViewModel(repository);
            filter.Load();

            var where = filter.GetWhereCondition();

            Assert.Multiple(() =>
                {
                    Assert.IsNotNull(where);
                    Assert.IsEmpty(where.ToString(true));
                }
            );
        }


        [Test]
        public void GetWhereCondition_SetupManufacturers_CorrectRestrictionInWhereCondition()
        {
            var filter = new GrinderFilterViewModel(repository);
            filter.Load();

            filter.Manufacturers[0].IsChecked = true;
            filter.Manufacturers[1].IsChecked = true;

            var where = filter.GetWhereCondition();

            Assert.Multiple(() =>
                {
                    Assert.IsNotNull(where);
                    Assert.AreEqual("[SKUManufacturerID] IN (1, 2)", where.ToString(true));
                }
            );
        }


        [TestCase(ManualGrinder.CLASS_NAME)]
        [TestCase(ElectricGrinder.CLASS_NAME)]
        public void GetWhereCondition_SetupType_CorrectRestrictionInWhereCondition(string className)
        {
            var filter = new GrinderFilterViewModel(repository);
            filter.Load();

            if (className.Equals(ElectricGrinder.CLASS_NAME))
            {
                filter.Type[0].IsChecked = true;
            }

            if (className.Equals(ManualGrinder.CLASS_NAME))
            {
                filter.Type[1].IsChecked = true;
            }            

            var where = filter.GetWhereCondition();

            Assert.Multiple(() =>
                {
                    Assert.IsNotNull(where);
                    Assert.AreEqual($"[ClassName] = N'{className}'", where.ToString(true));
                }
            );
        }


        [Test]
        public void GetWhereCondition_SetupPrice_CorrectRestrictionInWhereCondition()
        {
            var filter = new GrinderFilterViewModel(repository);
            filter.Load();

            filter.Prices[0].IsChecked = true;
            filter.Prices[1].IsChecked = false;
            filter.Prices[2].IsChecked = true;

            var where = filter.GetWhereCondition();

            Assert.Multiple(() =>
                {
                    Assert.IsNotNull(where);
                    Assert.AreEqual("(([SKUPrice] >= 0 AND [SKUPrice] < 50) OR ([SKUPrice] >= 500 AND [SKUPrice] < 5000))", where.ToString(true));
                }
            );
        }
        

        [Test]
        public void GetWhereCondition_SetupFullFilter_ReturnsCorrectWhereCondition()
        {
            var filter = new GrinderFilterViewModel(repository);
            filter.Load();

            filter.Manufacturers[0].IsChecked = true;
            filter.Manufacturers[1].IsChecked = true;

            filter.Prices[0].IsChecked = true;
            filter.Prices[1].IsChecked = true;

            filter.Type[0].IsChecked = true;
            filter.Type[1].IsChecked = false;

            var where = filter.GetWhereCondition();

            Assert.Multiple(() => 
                {
                    Assert.IsNotNull(where);
                    Assert.AreEqual("[SKUManufacturerID] IN (1, 2) AND (([SKUPrice] >= 0 AND [SKUPrice] < 50) OR ([SKUPrice] >= 50 AND [SKUPrice] < 500)) " +
                        $"AND [ClassName] = N'{ElectricGrinder.CLASS_NAME}'", where.ToString(true));
                }
            );
        }


        private void FakeManufacturers()
        {
            Fake<ManufacturerInfo, ManufacturerInfoProvider>().WithData(
                new ManufacturerInfo
                {
                    ManufacturerID = 1,
                    ManufacturerDisplayName = "Manufacturer1",
                    ManufacturerName = "Manufacturer1"
                },
                new ManufacturerInfo
                {
                    ManufacturerID = 2,
                    ManufacturerDisplayName = "Manufacturer2",
                    ManufacturerName = "Manufacturer2"
                }
            );
        }


        private List<SKUTreeNode> MockGrinders()
        {
            var skuInfo1 = Substitute.For<SKUInfo>();
            var skuInfo2 = Substitute.For<SKUInfo>();
            skuInfo1.SKUManufacturerID.Returns(1);            
            skuInfo2.SKUManufacturerID.Returns(2);            

            var manualGrinder = TreeNode.New<ManualGrinder>().With(x =>
            {
                x.DocumentName = "ManualGrinder";
                x.SKU = skuInfo1;
            });
            var electricGrinder = TreeNode.New<ElectricGrinder>().With(x =>
            {
                x.DocumentName = "ElectricGrinder";
                x.SKU = skuInfo2;
            });

            return new List<SKUTreeNode> { manualGrinder, electricGrinder };
        }
    }
}
