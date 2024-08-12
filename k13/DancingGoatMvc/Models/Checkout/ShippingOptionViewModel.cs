using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using CMS.Ecommerce;

namespace DancingGoat.Models.Checkout
{
    [Bind(Exclude = "ShippingOptions")]
    public class ShippingOptionViewModel
    {
        [Display(Name = "DancingGoatMvc.Shipping.ShippingOption")]
        public int ShippingOptionID { get; set; }


        public SelectList ShippingOptions { get; set; }


        public bool IsVisible { get; set; } = true;


        public ShippingOptionViewModel()
        {
        }


        public ShippingOptionViewModel(ShippingOptionInfo shippingOption, SelectList shippingOptions, bool isVisible = true)
        {
            ShippingOptions = shippingOptions;
            ShippingOptionID = shippingOption?.ShippingOptionID ?? 0;
            IsVisible = isVisible;
        }
    }
}