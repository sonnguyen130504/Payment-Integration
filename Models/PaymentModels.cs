namespace PaymentIntegration.Models;

public class VnpaySettings
{
    public const string SectionName = "VNPAY";
    public string TmnCode { get; set; } = string.Empty;
    public string HashSecret { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
    public string Version { get; set; } = "2.1.0";
    public string OrderType { get; set; } = "other";
}

public class SepaySettings
{
    public const string SectionName = "SePay";
    public string AccountNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string WebhookToken { get; set; } = string.Empty;
    public string ApiToken { get; set; } = string.Empty;
    public string OrderDescriptionPrefix { get; set; } = "TT DH";
}

public class PayosSettings
{
    public const string SectionName = "PayOS";
    public string ClientId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string ChecksumKey { get; set; } = string.Empty;
    public string ReturnUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
}

public class PaymentRequest
{
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
}
