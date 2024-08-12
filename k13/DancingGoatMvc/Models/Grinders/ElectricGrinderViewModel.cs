using CMS.DocumentEngine.Types.DancingGoatMvc;

using DancingGoat.Models.Products;

namespace DancingGoat.Models.Grinders
{
    public class ElectricGrinderViewModel : ITypedProductViewModel
    {
        public int Power { get; set; }


        public static ElectricGrinderViewModel GetViewModel(ElectricGrinder grinder)
        {
            return new ElectricGrinderViewModel
            {
                Power = grinder.ElectricGrinderPower
            };
        }
    }
}