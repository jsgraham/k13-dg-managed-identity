using System.Collections.Generic;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using DancingGoat.Models.Products;

namespace DancingGoat.Models.Manufacturers
{
    public class ManufacturerDetailViewModel
    {
        public string Name { get; }


        public IEnumerable<ProductListItemViewModel> Products { get; }


        public ManufacturerDetailViewModel(Manufacturer manufacturer, IEnumerable<ProductListItemViewModel> products)
        {
            Name = manufacturer.DocumentName;
            Products = products;
        }
    }
}