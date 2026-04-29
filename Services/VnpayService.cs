using VNPAY;
using VNPAY.Models;
using VNPAY.Models.Enums;
using PaymentIntegration.Interfaces;

namespace PaymentIntegration.Services;

public class VnpayService(IVnpayClient vnpayClient) : IPaymentService
{
    public string Provider => "vnpay";

    public Task<string> CreatePaymentUrl(string orderId, decimal amount, string description)
    {
        var request = new VnpayPaymentRequest
        {
            Money = (long)amount,
            Description = description,
            BankCode = BankCode.ANY,
            Language = DisplayLanguage.Vietnamese
        };

        var response = vnpayClient.CreatePaymentUrl(request);
        return Task.FromResult(response.Url);
    }
}
