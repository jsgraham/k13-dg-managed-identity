using System.ComponentModel.DataAnnotations;

using CMS.Ecommerce;

namespace DancingGoat.Models.Checkout
{
    public class CustomerViewModel
    {
        [Required]
        [Display(Name = "DancingGoatMvc.Firstname")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string FirstName { get; set; }


        [Required]
        [Display(Name = "DancingGoatMvc.Lastname")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "DancingGoatMvc.Email.Required")]
        [Display(Name = "DancingGoatMvc.Email")]
        [EmailAddress(ErrorMessage = "DancingGoatMvc.General.InvalidEmail")]
        [MaxLength(100, ErrorMessage = "DancingGoatMvc.News.LongEmail")]
        public string Email { get; set; }


        [Display(Name = "DancingGoatMvc.Phone")]
        [DataType(DataType.PhoneNumber)]
        [MaxLength(26, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string PhoneNumber { get; set; }


        [Display(Name = "DancingGoatMvc.CompanyName")]
        [MaxLength(200, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string Company { get; set; }


        [Display(Name = "DancingGoatMvc.OrganizationId")]
        [MaxLength(50, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string OrganizationID { get; set; }


        [Display(Name = "DancingGoatMvc.TaxRegistrationId")]
        [MaxLength(50, ErrorMessage = "DancingGoatMvc.General.MaximumInputLengthExceeded")]
        public string TaxRegistrationID { get; set; }


        public bool IsCompanyAccount { get; set; }


        public CustomerViewModel(CustomerInfo customer)
        {
            if (customer == null)
            {
                return;
            }

            FirstName = customer.CustomerFirstName;
            LastName = customer.CustomerLastName;
            Email = customer.CustomerEmail;
            PhoneNumber = customer.CustomerPhone;
            Company = customer.CustomerCompany;
            OrganizationID = customer.CustomerOrganizationID;
            TaxRegistrationID = customer.CustomerTaxRegistrationID;
            IsCompanyAccount = customer.CustomerHasCompanyInfo;
        }


        public CustomerViewModel()
        {
        }


        public void ApplyToCustomer(CustomerInfo customer, bool emailCanBeChanged)
        {
            customer.CustomerFirstName = FirstName;
            customer.CustomerLastName = LastName;
            customer.CustomerPhone = PhoneNumber;
            customer.CustomerCompany = Company;
            customer.CustomerOrganizationID = OrganizationID;
            customer.CustomerTaxRegistrationID = TaxRegistrationID;

            if (emailCanBeChanged)
            {
                customer.CustomerEmail = Email;
            }
        }
    }
}