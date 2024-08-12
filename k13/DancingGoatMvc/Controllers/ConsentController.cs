using System.Net;
using System.Threading;
using System.Web.Mvc;

using CMS.ContactManagement;
using CMS.DataProtection;
using CMS.Helpers;

using DancingGoat.Helpers.Generator;
using DancingGoat.Models.Consent;

namespace DancingGoat.Controllers
{
    public class ConsentController : Controller
    {
        private readonly ICurrentCookieLevelProvider cookieLevelProvider;
        private readonly IConsentAgreementService consentAgreementService;
        private readonly IConsentInfoProvider consentInfoProvider;


        public ConsentController(ICurrentCookieLevelProvider cookieLevelProvider, IConsentAgreementService consentAgreementService, IConsentInfoProvider consentInfoProvider)
        {
            this.cookieLevelProvider = cookieLevelProvider;
            this.consentAgreementService = consentAgreementService;
            this.consentInfoProvider = consentInfoProvider;
        }


        // GET: Consent
        public ActionResult Index()
        {
            var consent = consentInfoProvider.Get(TrackingConsentGenerator.CONSENT_NAME);
            if (consent != null)
            {
                var model = new ConsentViewModel
                {
                    ConsentShortText = consent.GetConsentText(Thread.CurrentThread.CurrentUICulture.Name).ShortText
                };

                var contact = ContactManagementContext.CurrentContact;
                if ((contact != null) && consentAgreementService.IsAgreed(contact, consent))
                {
                    model.IsConsentAgreed = true;
                }

                return PartialView("_TrackingConsent", model);
            }

            return new EmptyResult();
        }


        // POST: Consent/Agree
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Agree()
        {
            var resultStatus = HttpStatusCode.BadRequest;

            var consent = consentInfoProvider.Get(TrackingConsentGenerator.CONSENT_NAME);
            if (consent != null)
            {
                cookieLevelProvider.SetCurrentCookieLevel(CookieLevel.All);
                
                var contact = ContactManagementContext.CurrentContact;
                if (contact != null) 
                { 
                    consentAgreementService.Agree(contact, consent);
                }

                resultStatus = HttpStatusCode.OK;
            }

            // Redirect is handled on client by javascript
            return new HttpStatusCodeResult(resultStatus);
        }
    }
}