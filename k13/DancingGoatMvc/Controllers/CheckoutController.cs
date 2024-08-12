using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using CMS.Base;
using CMS.Ecommerce;
using CMS.Globalization;
using CMS.Helpers;

using Kentico.Content.Web.Mvc;

using DancingGoat.ActionSelectors;
using DancingGoat.Models.Checkout;
using DancingGoat.Repositories;
using DancingGoat.Services;

namespace DancingGoat.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly IShoppingService shoppingService;
        private readonly ICheckoutService checkoutService;
        private readonly IContactRepository contactRepository;
        private readonly IProductRepository productRepository;
        private readonly IPageUrlRetriever pageUrlRetriever;
        private readonly ISKUInfoProvider skuInfoProvider;
        private readonly ICountryInfoProvider countryInfoProvider;
        private readonly IStateInfoProvider stateInfoProvider;


        public CheckoutController(IShoppingService shoppingService, IContactRepository contactRepository, IProductRepository productRepository, 
            ICheckoutService checkoutService, IPageUrlRetriever pageUrlRetriever, ISKUInfoProvider skuInfoProvider, ICountryInfoProvider countryInfoProvider,
            IStateInfoProvider stateInfoProvider)
        {
            this.shoppingService = shoppingService;
            this.contactRepository = contactRepository;
            this.checkoutService = checkoutService;
            this.productRepository = productRepository;
            this.pageUrlRetriever = pageUrlRetriever;
            this.skuInfoProvider = skuInfoProvider;
            this.countryInfoProvider = countryInfoProvider;
            this.stateInfoProvider = stateInfoProvider;
        }


        public ActionResult ShoppingCart()
        {
            var viewModel = checkoutService.PrepareCartViewModel();

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult ShoppingCartCheckout()
        {
            var cart = shoppingService.GetCurrentShoppingCart();
            var validationErrors = ShoppingCartInfoProvider.ValidateShoppingCart(cart);

            cart.Evaluate();

            if (!validationErrors.Any())
            {
                var customer = GetCustomerOrCreateFromAuthenticatedUser();
                if (customer != null)
                {
                    shoppingService.SetCustomer(customer);
                }

                shoppingService.SaveCart();
                return RedirectToAction("DeliveryDetails");
            }

            // Fill model state with errors from the check result
            ProcessCheckResult(validationErrors);

            var viewModel = checkoutService.PrepareCartViewModel();

            return View("ShoppingCart", viewModel);
        }


        public ActionResult DeliveryDetails()
        {
            var cart = shoppingService.GetCurrentShoppingCart();
            if (cart.IsEmpty)
            {
                return RedirectToAction("ShoppingCart");
            }

            var viewModel = checkoutService.PrepareDeliveryDetailsViewModel();

            return View(viewModel);
        }


        public ActionResult PreviewAndPay()
        {
            if (shoppingService.GetCurrentShoppingCart().IsEmpty)
            {
                return RedirectToAction("ShoppingCart");
            }

            var viewModel = checkoutService.PreparePreviewViewModel();

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PreviewAndPay(PreviewViewModel model)
        {
            var cart = shoppingService.GetCurrentShoppingCart();

            if (cart.IsEmpty)
            {
                ModelState.AddModelError("cart.empty", ResHelper.GetString("DancingGoatMvc.Checkout.EmptyCartError"));

                var viewModel = checkoutService.PreparePreviewViewModel(model.PaymentMethod);
                return View("PreviewAndPay", viewModel);
            }

            if (!checkoutService.IsPaymentMethodValid(model.PaymentMethod.PaymentMethodID))
            {
                ModelState.AddModelError("PaymentMethod.PaymentMethodID", ResHelper.GetString("DancingGoatMvc.Payment.PaymentMethodRequired"));
            }
            else
            {
                shoppingService.SetPaymentOption(model.PaymentMethod.PaymentMethodID);
            }

            var validator = new CreateOrderValidator(cart, skuInfoProvider, countryInfoProvider, stateInfoProvider);

            if (!validator.Validate())
            {
                ProcessCheckResult(validator.Errors);
            }

            if (!ModelState.IsValid)
            {
                var viewModel = checkoutService.PreparePreviewViewModel(model.PaymentMethod);
                return View("PreviewAndPay", viewModel);
            }

            try
            {
                shoppingService.CreateOrder();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("cart.createordererror", ex.Message);
                var viewModel = checkoutService.PreparePreviewViewModel(model.PaymentMethod);
                return View("PreviewAndPay", viewModel);
            }

            return RedirectToAction("ThankYou");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult DeliveryDetails(DeliveryDetailsViewModel model)
        {
            // Check the selected shipping option
            bool isShippingNeeded = shoppingService.GetCurrentShoppingCart().IsShippingNeeded;
            if (isShippingNeeded && !checkoutService.IsShippingOptionValid(model.ShippingOption.ShippingOptionID))
            {
                ModelState.AddModelError("ShippingOption.ShippingOptionID", ResHelper.GetString("DancingGoatMvc.Shipping.ShippingOptionRequired"));
            }

            // Check if the billing address's country and state are valid
            var countryStateViewModel = model.BillingAddress.BillingAddressCountryStateSelector;
            if (!checkoutService.IsCountryValid(countryStateViewModel.CountryID))
            {
                countryStateViewModel.CountryID = 0;
                ModelState.AddModelError("BillingAddress.BillingAddressCountryStateSelector.CountryID", ResHelper.GetString("DancingGoatMvc.Address.CountryIsRequired"));
            }
            else if (!checkoutService.IsStateValid(countryStateViewModel.CountryID, countryStateViewModel.StateID))
            {
                countryStateViewModel.StateID = 0;
                ModelState.AddModelError("BillingAddress.BillingAddressCountryStateSelector.StateID", ResHelper.GetString("DancingGoatMvc.Address.StateIsRequired"));
            }

            if (!ModelState.IsValid)
            {
                var viewModel = checkoutService.PrepareDeliveryDetailsViewModel(model.Customer, model.BillingAddress, model.ShippingOption);

                return View(viewModel);
            }

            var customer = GetCustomerOrCreateFromAuthenticatedUser() ?? new CustomerInfo();
            bool emailCanBeChanged = !User.Identity.IsAuthenticated || string.IsNullOrWhiteSpace(customer.CustomerEmail);
            model.Customer.ApplyToCustomer(customer, emailCanBeChanged);
            shoppingService.SetCustomer(customer);

            var modelAddressID = model.BillingAddress.BillingAddressSelector?.AddressID ?? 0;
            var billingAddress = checkoutService.GetAddress(modelAddressID) ?? new AddressInfo();

            model.BillingAddress.ApplyTo(billingAddress);
            billingAddress.AddressPersonalName = $"{customer.CustomerFirstName} {customer.CustomerLastName}";

            shoppingService.SetBillingAddress(billingAddress);
            shoppingService.SetShippingOption(model.ShippingOption?.ShippingOptionID ?? 0);

            return RedirectToAction("PreviewAndPay");
        }


        public ActionResult ThankYou()
        {
            var companyContact = contactRepository.GetCompanyContact();

            var viewModel = new ThankYouViewModel
            {
                Phone = companyContact.Phone
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult AddItem(CartItemUpdateModel item)
        {
            if (ModelState.IsValid)
            {
                shoppingService.AddItemToCart(item.SKUID, item.Units);
            }

            return RedirectToAction("ShoppingCart");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult UpdateItem(CartItemUpdateModel item)
        {
            if (ModelState.IsValid)
            {
                shoppingService.UpdateItemQuantity(item.ID, item.Units);
            }
            else
            {
                // Add an item error and save ViewData so that the ShoppingCart action can show validation errors
                var key = item.ID.ToString();
                ModelState.Add(key, ModelState["Units"]);
            }

            var cartViewModel = checkoutService.PrepareCartViewModel();
            return View("ShoppingCart", cartViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ButtonNameAction]
        [ValidateInput(false)]
        public ActionResult RemoveItem(CartItemUpdateModel item)
        {
            shoppingService.RemoveItemFromCart(item.ID);

            var cartViewModel = checkoutService.PrepareCartViewModel();
            return View("ShoppingCart", cartViewModel);
        }


        [HttpPost]
        public JsonResult CustomerAddress(int addressID)
        {
            var address = checkoutService.GetCustomerAddress(addressID); 

            if (address == null)
            {
                return null;
            }

            var responseModel = new
            {
                Line1 = address.AddressLine1,
                Line2 = address.AddressLine2,
                City = address.AddressCity,
                PostalCode = address.AddressZip,
                CountryID = address.AddressCountryID,
                StateID = address.AddressStateID,
                PersonalName = address.AddressPersonalName
            };

            return Json(responseModel);
        }


        public ActionResult ItemDetail(int skuId)
        {
            var product = productRepository.GetProductForSKU(skuId);

            if (product == null)
            {
                return HttpNotFound();
            }

            var path = pageUrlRetriever.Retrieve(product).RelativePath;

            return Redirect(path);
        }


        [HttpPost]
        public JsonResult CountryStates(int countryId)
        {
            var responseModel = checkoutService.GetCountryStates(countryId)
                .Select(s => new
                {
                    id = s.StateID,
                    name = HTMLHelper.HTMLEncode(s.StateDisplayName)
                });

            return Json(responseModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PaymentChanged(int? paymentId)
        {
            var cart = shoppingService.GetCurrentShoppingCart();
            shoppingService.SetPaymentOption(paymentId ?? 0);

            var viewModel = new CartViewModel(cart);

            return PartialView("_ShoppingCartTotals", viewModel);
        }


        private void ProcessCheckResult(IEnumerable<IValidationError> validationErrors)
        {
            var itemErrors = validationErrors
                .OfType<ShoppingCartItemValidationError>()
                .GroupBy(g => g.SKUId);

            foreach (var errorGroup in itemErrors)
            {
                var errors = errorGroup
                    .Select(e => e.GetMessage())
                    .Join(" ");

                ModelState.AddModelError(errorGroup.Key.ToString(), errors);
            }
        }


        private CustomerInfo GetCustomerOrCreateFromAuthenticatedUser()
        {
            var cart = shoppingService.GetCurrentShoppingCart();
            var customer = cart.Customer;

            if (customer != null)
            {
                return customer;
            }

            var user = cart.User;

            return user != null ? CustomerHelper.MapToCustomer(user) : null;
        }
    }
}