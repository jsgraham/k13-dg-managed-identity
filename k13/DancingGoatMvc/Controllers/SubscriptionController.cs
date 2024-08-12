using System;
using System.Web.Mvc;

using CMS.Helpers;
using CMS.Newsletters;
using CMS.SiteProvider;

using DancingGoat.Models.Subscription;

namespace DancingGoat.Web.Controllers
{
    public class SubscriptionController : Controller
    {
        private readonly ISubscriptionService subscriptionService;
        private readonly ISubscriptionApprovalService subscriptionApprovalService;
        private readonly IUnsubscriptionProvider unsubscriptionProvider;
        private readonly IEmailHashValidator emailHashValidator;
        private readonly IIssueInfoProvider issueInfoProvider;
        private readonly INewsletterInfoProvider newsletterInfoProvider;


        public SubscriptionController(ISubscriptionService subscriptionService, ISubscriptionApprovalService subscriptionApprovalService, IUnsubscriptionProvider unsubscriptionProvider,
            IEmailHashValidator emailHashValidator, IIssueInfoProvider issueInfoProvider, INewsletterInfoProvider newsletterInfoProvider)
        {
            this.subscriptionService = subscriptionService;
            this.subscriptionApprovalService = subscriptionApprovalService;
            this.unsubscriptionProvider = unsubscriptionProvider;
            this.emailHashValidator = emailHashValidator;
            this.issueInfoProvider = issueInfoProvider;
            this.newsletterInfoProvider = newsletterInfoProvider;
        }


        // GET: Subscription/ConfirmSubscription
        [ValidateInput(false)]
        public ActionResult ConfirmSubscription(ConfirmSubscriptionModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionInvalidLink"));

                return View(model);
            }

            DateTime parsedDateTime = DateTimeHelper.ZERO_TIME;

            // Parse date and time from query string, if present
            if (!string.IsNullOrEmpty(model.DateTime) && !DateTimeUrlFormatter.TryParse(model.DateTime, out parsedDateTime))
            {
                ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionInvalidDateTime"));

                return View(model);
            }

            var result = subscriptionApprovalService.ApproveSubscription(model.SubscriptionHash, false, SiteContext.CurrentSiteName, parsedDateTime);

            switch (result)
            {
                case ApprovalResult.Success:
                    model.ConfirmationResult = ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionSucceeded");
                    break;

                case ApprovalResult.AlreadyApproved:
                    model.ConfirmationResult = ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionAlreadyConfirmed");
                    break;

                case ApprovalResult.TimeExceeded:
                    ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionTimeExceeded"));
                    break;

                case ApprovalResult.NotFound:
                    ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionInvalidLink"));
                    break;

                default:
                    ModelState.AddModelError(String.Empty, ResHelper.GetString("DancingGoatMvc.News.ConfirmSubscriptionFailed"));

                    break;
            }

            return View(model);
        }


        // GET: Subscription/Unsubscribe
        [ValidateInput(false)]
        public ActionResult Unsubscribe(UnsubscriptionModel model)
        {
            string invalidUrlMessage = ResHelper.GetString("DancingGoatMvc.News.InvalidUnsubscriptionLink");

            if (ModelState.IsValid)
            {
                bool emailIsValid = emailHashValidator.ValidateEmailHash(model.Hash, model.Email);
                
                if (emailIsValid)
                {
                    try
                    {
                        var issue = issueInfoProvider.Get(model.IssueGuid, SiteContext.CurrentSiteID);

                        if (model.UnsubscribeFromAll)
                        {
                            // Unsubscribes if not already unsubscribed
                            if (!unsubscriptionProvider.IsUnsubscribedFromAllNewsletters(model.Email))
                            {
                                subscriptionService.UnsubscribeFromAllNewsletters(model.Email, issue?.IssueID);
                            }

                            model.UnsubscriptionResult = ResHelper.GetString("DancingGoatMvc.News.UnsubscribedAll");
                        }
                        else
                        {
                            var newsletter = newsletterInfoProvider.Get(model.NewsletterGuid, SiteContext.CurrentSiteID);
                            if (newsletter != null)
                            {
                                // Unsubscribes if not already unsubscribed
                                if (!unsubscriptionProvider.IsUnsubscribedFromSingleNewsletter(model.Email, newsletter.NewsletterID))
                                {
                                    subscriptionService.UnsubscribeFromSingleNewsletter(model.Email, newsletter.NewsletterID, issue?.IssueID);
                                }

                                model.UnsubscriptionResult = ResHelper.GetString("DancingGoatMvc.News.Unsubscribed");
                            }
                            else
                            {
                                ModelState.AddModelError(String.Empty, invalidUrlMessage);
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                        ModelState.AddModelError(String.Empty, invalidUrlMessage);
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, invalidUrlMessage);
                }
            }
            else
            {
                ModelState.AddModelError(String.Empty, invalidUrlMessage);
            }

            return View(model);
        }
    }
}