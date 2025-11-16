using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

using Stripe.Checkout;
using System.Diagnostics;

namespace Givehub.Controllers;

public class PaymentController : Controller
{
    private readonly StripeSettings _stripeSettings;
    private readonly DB _db;
    //private readonly Helper _helper;

    public PaymentController(IOptions<StripeSettings> stripeSettings, DB db)
    {
        _stripeSettings = stripeSettings.Value;
        _db = db;
    }

    public IActionResult Payment()
    {
        return View();
    }

    public IActionResult CreateCheckoutSesssion(string amount)
    {
        var currency = "myr";
        var successUrl = "http://localhost:7198/Payment/Success";
        var cancelUrl = "http://localhost:7198/Payment/Cancel";
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;

        var options = new SessionCreateOptions
        {
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = currency,
                        UnitAmount = Convert.ToInt32(amount) * 100,
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Donation",
                            Description = "Thank you for your donation!"
                        }
                    },
                Quantity = 1
                }
            },

        
            Mode = "payment",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl
        };

        var service = new SessionService();
        var session = service.Create(options);
        return Redirect(session.Url);
    }

    public async Task<IActionResult> success()
    {
        return View("Index");
    }

    public IActionResult cancel()
    {
        return View("Cancel");
    }

}
