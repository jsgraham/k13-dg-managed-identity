using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;

using CMS.Newsletters;
using CMS.SiteProvider;
using CMS.Helpers;

using DancingGoat.Controllers.Widgets;
using DancingGoat.Models.Widgets;

using Kentico.Membership;
using Kentico.PageBuilder.Web.Mvc;

using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

[assembly: RegisterWidget(NewsletterSubscriptionWidgetController.IDENTIFIER, typeof(NewsletterSubscriptionWidgetController), "{$dancinggoatmvc.widget.newslettersubscription.name$}", Description = "{$dancinggoatmvc.widget.newslettersubscription.description$}", IconClass = "	icon-message")]

namespace DancingGoat.Controllers.Widgets
{
    /// <summary>
    /// Controller for newsletter subscription widget.
    /// </summary>
    public class NewsletterSubscriptionWidgetController : WidgetController<NewsletterSubscriptionProperties>
    {
        /// <summary>
        /// Widget identifier.
        /// </summary>
        public const string IDENTIFIER = "DancingGoat.NewsletterSubscriptionWidget";


        private SubscribeSettings mNewsletterSubscriptionSettings;
        private readonly ISubscriptionService subscriptionService;
        private readonly IContactProvider contactProvider;
        private readonly INewsletterInfoProvider newsletterInfoProvider;
        private readonly IComponentPropertiesRetriever componentPropertiesRetriever;

        private SubscribeSettings NewsletterSubscriptionSettings
        {
            get
            {
                return mNewsletterSubscriptionSettings ?? (mNewsletterSubscriptionSettings = new SubscribeSettings
                {
                    AllowOptIn = true,
                    RemoveUnsubscriptionFromNewsletter = true,
                    RemoveAlsoUnsubscriptionFromAllNewsletters = true,
                    SendConfirmationEmail = true
                });
            }
        }


        private KenticoUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().Get<KenticoUserManager>();
            }
        }


        /// <summary>
        /// Creates an instance of <see cref="NewsletterSubscriptionWidgetController"/> class.
        /// </summary>
        /// <param name="subscriptionService">Service for newsletter subscription.</param>
        /// <param name="contactProvider">Provider for contact retrieving.</param>
        /// <param name="newsletterInfoProvider">Provider for <see cref="NewsletterInfo"/> management.</param>
        public NewsletterSubscriptionWidgetController(
            ISubscriptionService subscriptionService, 
            IContactProvider contactProvider, 
            INewsletterInfoProvider newsletterInfoProvider,
            IComponentPropertiesRetriever componentPropertiesRetriever)
        {
            this.subscriptionService = subscriptionService;
            this.contactProvider = contactProvider;
            this.newsletterInfoProvider = newsletterInfoProvider;
            this.componentPropertiesRetriever = componentPropertiesRetriever;
        }


        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Handle authenticated user
                return PartialView("Widgets/NewsletterSubscriptionWidget/_SubscribeAuthenticated");
            }

            return PartialView("Widgets/NewsletterSubscriptionWidget/_Subscribe", new SubscribeModel());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Subscribe(SubscribeModel model)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_Subscribe", model);
            }

            var properties = componentPropertiesRetriever.Retrieve<NewsletterSubscriptionProperties>();
            var newsletter = newsletterInfoProvider.Get(properties.Newsletter, SiteContext.CurrentSiteID);
            var contact = contactProvider.GetContactForSubscribing(model.Email);

            string resultMessage;
            if (!subscriptionService.IsMarketable(contact, newsletter))
            {
                subscriptionService.Subscribe(contact, newsletter, NewsletterSubscriptionSettings);

                // The subscription service is configured to use double opt-in, but newsletter must allow for it
                resultMessage = ResHelper.GetString(newsletter.NewsletterEnableOptIn ? "DancingGoatMvc.News.ConfirmationSent" : "DancingGoatMvc.News.Subscribed");
            }
            else
            {
                resultMessage = ResHelper.GetString("DancingGoatMvc.News.AlreadySubscribed");
            }

            return Content(resultMessage);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult SubscribeAuthenticated()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Index();
            }

            var user = UserManager.FindByName(User.Identity.Name);
            var contact = contactProvider.GetContactForSubscribing(user.Email, user.FirstName, user.LastName);
            var properties = componentPropertiesRetriever.Retrieve<NewsletterSubscriptionProperties>();
            var newsletter = newsletterInfoProvider.Get(properties.Newsletter, SiteContext.CurrentSiteID);

            string resultMessage;
            if (!subscriptionService.IsMarketable(contact, newsletter))
            {
                subscriptionService.Subscribe(contact, newsletter, NewsletterSubscriptionSettings);

                // The subscription service is configured to use double opt-in, but newsletter must allow for it
                resultMessage = ResHelper.GetString(newsletter.NewsletterEnableOptIn ? "DancingGoatMvc.News.ConfirmationSent" : "DancingGoatMvc.News.Subscribed");
            }
            else
            {
                resultMessage = ResHelper.GetString("DancingGoatMvc.News.AlreadySubscribed");
            }

            return Content(resultMessage);
        }
    }
}