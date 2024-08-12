using CMS.Helpers;

using DancingGoat.Helpers.Generator;

namespace DancingGoat.Models.Consent
{
    public class ConsentViewModel
    {
        public string ConsentNotAgreedText => GetString("TrackingConsent.NotAgreedText");
        public string ConsentAgreedText => GetString("TrackingConsent.AgreedText");
        public string AgreeButtonText => GetString("TrackingConsent.AgreeButtonText");
        public string PrivacyPageLinkText => GetString("PrivacyPageLinkText");
        public string ConsentShortText { get; set; }
        public bool IsConsentAgreed { get; set; }


        private string GetString(string resourceString)
        {
            return ResHelper.GetString($"DancingGoatMvc.DataProtection.{resourceString}");
        }
    }
}