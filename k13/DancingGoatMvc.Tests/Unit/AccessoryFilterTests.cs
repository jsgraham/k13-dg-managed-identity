using System.Linq;

using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Tests;

using DancingGoat.Models.Accessories;

using NUnit.Framework;

namespace DancingGoat.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class AccessoryFilterTests : UnitTests
    {
        [Test]
        public void Load_FilterContainsCorrectTypes()
        {
            var filter = new AccessoryFilterViewModel();
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
            var filter = new AccessoryFilterViewModel();
            filter.Load();

            var where = filter.GetWhereCondition();

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(where);
                Assert.IsEmpty(where.ToString(true));
            }
            );
        }


        [TestCase(Tableware.CLASS_NAME)]
        [TestCase(FilterPack.CLASS_NAME)]
        public void GetWhereCondition_SetupType_CorrectRestrictionInWhereCondition(string className)
        {
            var filter = new AccessoryFilterViewModel();
            filter.Load();

            if (className.Equals(Tableware.CLASS_NAME))
            {
                filter.Type[0].IsChecked = true;
            }

            if (className.Equals(FilterPack.CLASS_NAME))
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
    }
}
