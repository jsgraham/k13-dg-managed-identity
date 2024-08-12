using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.DocumentEngine.Types.DancingGoatMvc;

using Kentico.Content.Web.Mvc.Routing;

using DancingGoat.Controllers;
using DancingGoat.Models.Contacts;
using DancingGoat.Repositories;

[assembly: RegisterPageRoute(Contacts.CLASS_NAME, typeof(ContactsController))]

namespace DancingGoat.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ICafeRepository cafeRepository;
        private readonly IContactRepository contactRepository;
        private readonly ICountryRepository countryRepository;


        public ContactsController(ICafeRepository cafeRepository,
            IContactRepository contactRepository, 
            ICountryRepository countryRepository)
        {
            this.countryRepository = countryRepository;
            this.cafeRepository = cafeRepository;
            this.contactRepository = contactRepository;
        }


        public ActionResult Index()
        {
            var model = GetIndexViewModel();

            return View(model);
        }


        private IndexViewModel GetIndexViewModel()
        {
            var cafes = cafeRepository.GetCompanyCafes(ContentItemIdentifiers.CAFES, 4);

            return new IndexViewModel
            {
                CompanyContact = GetCompanyContactModel(),
                CompanyCafes = GetCompanyCafesModel(cafes)
            };
        }


        private ContactViewModel GetCompanyContactModel()
        {
            return ContactViewModel.GetViewModel(contactRepository.GetCompanyContact(), countryRepository);
        }


        private List<ContactViewModel> GetCompanyCafesModel(IEnumerable<Cafe> cafes)
        {
            return cafes.Select(cafe => ContactViewModel.GetViewModel(cafe, countryRepository)).ToList();
        }
    }
}