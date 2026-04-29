using System.Security.Cryptography;
using System.Text;
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
        // For SePay, we return a URL that triggers the local checkout endpoint which generates the signed HTML form
        var checkoutUrl = $"/api/payment/sepay?orderId={orderId}&amount={amount:0}";
        return Task.FromResult(checkoutUrl);
    }

    public string CreateCheckoutHtml(PaymentRequest request)
    {
        var fields = new Dictionary<string, string>
        {
            { "merchant", _settings.MerchantId },
            { "currency", "VND" },
            { "order_amount", ((long)request.Amount).ToString() },
            { "operation", "PURCHASE" },
            { "order_description", request.Description },
            { "order_invoice_number", request.OrderId },
            { "customer_id", request.OrderId },
            { "success_url", $"http://localhost:5200/success.html?gateway=sepay&orderId={request.OrderId}" },
            { "error_url", $"http://localhost:5200/error.html?gateway=sepay&orderId={request.OrderId}" },
            { "cancel_url", $"http://localhost:5200/error.html?gateway=sepay&orderId={request.OrderId}" }
        };

        var signature = SignFields(fields, _settings.SecretKey);
        fields.Add("signature", signature);

        var sb = new StringBuilder();
        sb.AppendLine("<html><head><title>Redirecting to SePay...</title></head><body onload=\"document.forms[0].submit();\">");
        sb.AppendLine("<form method=\"POST\" action=\"https://pay-sandbox.sepay.vn/v1/checkout/init\">");
        
        foreach (var field in fields)
        {
            sb.AppendLine($"<input type=\"hidden\" name=\"{field.Key}\" value=\"{field.Value}\">");
        }
        
        sb.AppendLine("</form></body></html>");

        return sb.ToString();
    }

    private static string SignFields(Dictionary<string, string> fields, string secretKey)
    {
        var signedFieldsKeys = new[]
        {
            "merchant", "currency", "order_amount", "operation", "order_description",
            "order_invoice_number", "customer_id", "success_url", "error_url", "cancel_url"
        };

        var signedValues = new List<string>();

        foreach (var key in signedFieldsKeys)
        {
            if (fields.TryGetValue(key, out var val))
            {
                signedValues.Add($"{key}={val}");
            }
        }

        var dataToSign = string.Join(",", signedValues);

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
        return Convert.ToBase64String(hash);
    }
}
