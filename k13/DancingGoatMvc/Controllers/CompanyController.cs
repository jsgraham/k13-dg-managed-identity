using System.Linq;
using System.Web.Mvc;

using DancingGoat.Models.Contacts;
using DancingGoat.Models.SocialLinks;
using DancingGoat.Repositories;

using Kentico.Content.Web.Mvc;

namespace DancingGoat.Controllers
{
    public class CompanyController : Controller
    {
        private readonly IContactRepository contactRepository;
        private readonly ICountryRepository countryRepository;
        private readonly ISocialLinkRepository socialLinkRepository;
        private readonly IPageAttachmentUrlRetriever attachmentUrlRetriever;


        public CompanyController(ISocialLinkRepository socialLinkRepository,
            IContactRepository contactRepository,
            ICountryRepository countryRepository, 
            IPageAttachmentUrlRetriever attachmentUrlRetriever)
        {
            this.countryRepository = countryRepository;
            this.socialLinkRepository = socialLinkRepository;
            this.contactRepository = contactRepository;
            this.attachmentUrlRetriever = attachmentUrlRetriever;
        }


        [ChildActionOnly]
        [ValidateInput(false)]
        public ActionResult CompanyAddress()
        {
            var address = GetCompanyContactModel();

            return PartialView("_Address", address);
        }


        [ChildActionOnly]
        [ValidateInput(false)]
        public ActionResult CompanySocialLinks()
        {
            var socialLinks = socialLinkRepository.GetSocialLinks();

            return PartialView("_SocialLinks", socialLinks.Select(link => SocialLinkViewModel.GetViewModel(link, attachmentUrlRetriever)));
        }


        private ContactViewModel GetCompanyContactModel()
        {
            return ContactViewModel.GetViewModel(contactRepository.GetCompanyContact(), countryRepository);
        }
    }
}