using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Content.Web.Mvc;

using DancingGoat.Models.Contacts;
using DancingGoat.Repositories;

namespace DancingGoat.Models.Cafes
{
    public class CafeViewModel
    {
        public string PhotoPath { get; set; }


        public string Note { get; set; }


        public ContactViewModel Contact { get; set; }


        public static CafeViewModel GetViewModel(Cafe cafe, ICountryRepository countryRepository, IPageAttachmentUrlRetriever attachmentUrlRetriever)
        {
            return new CafeViewModel
            {
                PhotoPath = cafe.Fields.Photo == null ? null : attachmentUrlRetriever.Retrieve(cafe.Fields.Photo).RelativePath,
                Note = cafe.Fields.AdditionalNotes,
                Contact = ContactViewModel.GetViewModel(cafe, countryRepository)
            };
        }
    }
}