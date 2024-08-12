using System;
using System.Collections.Generic;

using CMS.Ecommerce;

namespace DancingGoat.Models.Orders
{
    public class OrderDetailViewModel
    {
        private readonly string currencyFormatString;


        public string InvoiceNumber { get; set; }

        public decimal TotalPrice { get; set; }

        public string StatusName { get; set; }

        public OrderAddressViewModel OrderAddress { get; set; }

        public IEnumerable<OrderItemViewModel> OrderItems { get; set; }


        public OrderDetailViewModel(string currencyFormatString)
        {
            if (String.IsNullOrEmpty(currencyFormatString))
            {
                throw new ArgumentException($"{nameof(currencyFormatString)} is not defined.");
            }

            this.currencyFormatString = currencyFormatString;
        }

        public string FormatPrice(decimal price)
        {
            return String.Format(currencyFormatString, price);
        }
    }
}