namespace PaymentIntegration.Interfaces;

public interface IPaymentService
{
    string Provider { get; }
    Task<string> CreatePaymentUrl(string orderId, decimal amount, string description);
}
