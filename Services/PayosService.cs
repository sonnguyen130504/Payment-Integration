using Microsoft.Extensions.Options;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PaymentIntegration.Interfaces;
using PaymentIntegration.Models;

namespace PaymentIntegration.Services;

public class PayosService : IPaymentService
{
    private readonly PayOSClient _payOS;
    private readonly PayosSettings _settings;

    public string Provider => "payos";

    public PayosService(IOptions<PayosSettings> settings)
    {
        _settings = settings.Value;
        _payOS = new PayOSClient(_settings.ClientId, _settings.ApiKey, _settings.ChecksumKey);
    }

    public async Task<string> CreatePaymentUrl(string orderId, decimal amount, string description)
    {
        if (!long.TryParse(orderId, out long orderCode))
        {
            if (orderId.StartsWith("PAY") && long.TryParse(orderId.Substring(3), out long parsed))
            {
                orderCode = parsed;
            }
            else
            {
                orderCode = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }

        var paymentRequest = new CreatePaymentLinkRequest
        {
            OrderCode = orderCode,
            Amount = (int)amount,
            Description = description,
            CancelUrl = _settings.CancelUrl,
            ReturnUrl = _settings.ReturnUrl,
            Items = new List<PaymentLinkItem> 
            { 
                new PaymentLinkItem 
                { 
                    Name = description, 
                    Quantity = 1, 
                    Price = (int)amount 
                } 
            }
        };

        var paymentResponse = await _payOS.PaymentRequests.CreateAsync(paymentRequest);
        return paymentResponse.CheckoutUrl;
    }

    public async Task<PayOS.Models.Webhooks.WebhookData> VerifyWebhook(PayOS.Models.Webhooks.Webhook webhook)
    {
        return await _payOS.Webhooks.VerifyAsync(webhook);
    }
}
