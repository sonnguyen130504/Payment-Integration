using Microsoft.Extensions.Options;
using PaymentIntegration.Interfaces;
using PaymentIntegration.Models;

namespace PaymentIntegration.Services;

public class SepayService(IOptions<SepaySettings> settings) : IPaymentService
{
    private readonly SepaySettings _settings = settings.Value;

    public string Provider => "sepay";

    public Task<string> CreatePaymentUrl(string orderId, decimal amount, string description)
    {
        // For SePay, we return a URL that would typically trigger an auto-submit form or a checkout page.
        // In this demo, we'll return a direct URL to our local checkout endpoint.
        var checkoutUrl = $"/api/payment/sepay?orderId={orderId}&amount={amount:0}";
        return Task.FromResult(checkoutUrl);
    }

    public string CreateCheckoutHtml(PaymentRequest request)
    {
        // Existing logic for generating the HTML form
        var html = $@"
        <html>
        <body onload='document.forms[0].submit()'>
            <form action='{_settings.BaseUrl}' method='POST'>
                <input type='hidden' name='merchant_id' value='{_settings.MerchantId}' />
                <input type='hidden' name='order_id' value='{request.OrderId}' />
                <input type='hidden' name='amount' value='{request.Amount:0}' />
                <input type='hidden' name='description' value='{request.Description}' />
                <input type='hidden' name='success_url' value='{_settings.SuccessUrl}' />
            </form>
        </body>
        </html>";
        return html;
    }
}
