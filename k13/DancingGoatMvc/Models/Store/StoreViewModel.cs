using System.Collections.Generic;

using DancingGoat.Models.Manufacturers;
using DancingGoat.Models.Products;

namespace DancingGoat.Models.Store
{
    public class StoreViewModel
    {
        public IEnumerable<ProductListItemViewModel> FeaturedProducts { get; set; }

        public IEnumerable<ProductListItemViewModel> HotTipProducts { get; set; }

        public IEnumerable<ManufactureListItemViewModel> Manufacturers { get; set; }
    }
}