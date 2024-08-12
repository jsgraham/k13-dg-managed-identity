using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using CMS.Ecommerce;
using CMS.Globalization;

using DancingGoat.Repositories;

namespace DancingGoat.Models.Checkout
{
    [Bind(Exclude = "Countries")]
    public class BillingAddressViewModel
    {
        [Required]
        [Display(Name = "DancingGoatMvc.Address.Line1")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string BillingAddressLine1 { get; set; }



        [Display(Name = "DancingGoatMvc.Address.Line2")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string BillingAddressLine2 { get; set; }


        [Required]
        [Display(Name = "DancingGoatMvc.Address.City")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string BillingAddressCity { get; set; }


        [Required]
        [Display(Name = "DancingGoatMvc.Address.Zip")]
        [MaxLength(20, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string BillingAddressPostalCode { get; set; }
        

        public CountryStateViewModel BillingAddressCountryStateSelector { get; set; }


        public AddressSelectorViewModel BillingAddressSelector { get; set; }


        public SelectList Countries { get; set; }


        public string BillingAddressState { get; set; }


        public string BillingAddressCountry { get; set; }


        public BillingAddressViewModel()
        {
        }


        public BillingAddressViewModel(AddressInfo address, SelectList countries, ICountryRepository countryRepository, SelectList addresses = null)
        {
            if (address != null)
            {
                if (countryRepository == null)
                {
                    throw new ArgumentNullException(nameof(countryRepository));
                }

                BillingAddressLine1 = address.AddressLine1;
                BillingAddressLine2 = address.AddressLine2;
                BillingAddressCity = address.AddressCity;
                BillingAddressPostalCode = address.AddressZip;
                BillingAddressState = countryRepository.GetState(address.AddressStateID)?.StateDisplayName ?? String.Empty;
                BillingAddressCountry = countryRepository.GetCountry(address.AddressCountryID)?.CountryDisplayName ?? String.Empty;
                Countries = countries;
            }

            BillingAddressCountryStateSelector = new CountryStateViewModel
            {
                Countries = countries,
                CountryID = address?.AddressCountryID ?? 0,
                StateID = address?.AddressStateID ?? 0
            };

            BillingAddressSelector = new AddressSelectorViewModel
            {
                Addresses = addresses,
                AddressID = address?.AddressID ?? 0
            };
        }


        public void ApplyTo(AddressInfo address)
        {
            address.AddressLine1 = BillingAddressLine1;
            address.AddressLine2 = BillingAddressLine2;
            address.AddressCity = BillingAddressCity;
            address.AddressZip = BillingAddressPostalCode;
            address.AddressCountryID = BillingAddressCountryStateSelector.CountryID;
            address.AddressStateID = BillingAddressCountryStateSelector.StateID ?? 0;
        }
    }
}