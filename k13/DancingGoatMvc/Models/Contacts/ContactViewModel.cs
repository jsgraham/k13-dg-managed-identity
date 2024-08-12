using CMS.Helpers;

using DancingGoat.Repositories;

namespace DancingGoat.Models.Contacts
{
    public class ContactViewModel
    {
        public string Name { get; set; }


        public string Phone { get; set; }


        public string Email { get; set; }


        public string ZIP { get; set; }


        public string Street { get; set; }


        public string City { get; set; }


        public string Country { get; set; }


        public string CountryCode { get; set; }


        public string State { get; set; }


        public string StateCode { get; set; }


        public ContactViewModel(IContact contact)
        {
            Name = contact.Name;
            Phone = contact.Phone;
            Email = contact.Email;
            ZIP = contact.ZIP;
            Street = contact.Street;
            City = contact.City;
        }


        public static ContactViewModel GetViewModel(IContact contact, ICountryRepository countryRepository)
        {
            var countryStateName = CountryStateName.Parse(contact.Country);
            var country = countryRepository.GetCountry(countryStateName.CountryName);
            var state = countryRepository.GetState(countryStateName.StateName);

            var model = new ContactViewModel(contact)
            {
                CountryCode = country.CountryTwoLetterCode,
                Country = ResHelper.LocalizeString(country.CountryDisplayName)
            };

            if (state != null)
            {
                model.StateCode = state.StateName;
                model.State = ResHelper.LocalizeString(state.StateDisplayName);
            }

            return model;
        }
    }
}