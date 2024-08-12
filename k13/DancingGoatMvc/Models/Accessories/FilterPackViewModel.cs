using CMS.DocumentEngine.Types.DancingGoatMvc;

using DancingGoat.Models.Products;

namespace DancingGoat.Models.Accessories
{
    public class FilterPackViewModel : ITypedProductViewModel
    {
        public int Quantity { get; set; }

        public static FilterPackViewModel GetViewModel(FilterPack filterPack)
        {
            return new FilterPackViewModel
            {
                Quantity = filterPack.FilterPackQuantity
            };
        }
    }
}