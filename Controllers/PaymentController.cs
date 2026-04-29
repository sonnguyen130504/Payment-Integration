using Microsoft.AspNetCore.Mvc;
using PaymentIntegration.Models;
using PaymentIntegration.Services;
using VNPAY;
using VNPAY.Models.Exceptions;

namespace PaymentIntegration.Controllers;

/// <summary>
/// Controller for handling various payment gateway integrations.
/// </summary>
public class PaymentController(
    VnpayService vnpayService, 
    SepayService sepayService, 
    PayosService payosService,
    IVnpayClient vnpayClient,
    ILogger<PaymentController> logger) : BaseController
{
    /// <summary>
    /// Creates a VNPay payment URL.
    /// </summary>
    [HttpPost("vnpay")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> CreateVnpayPayment([FromBody] PaymentIntegration.Models.PaymentRequest request)
    {
        var url = await vnpayService.CreatePaymentUrl(request.OrderId, request.Amount, request.Description);
        return HandleResult(ApiResponse<string>.SuccessResult(url));
    }

    /// <summary>
    /// Handles the VNPay callback after user payment.
    /// </summary>
    [HttpGet("vnpay-callback")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult VnpayCallback()
    {
        try
        {
            var paymentResult = vnpayClient.GetPaymentResult(Request);
            return Redirect($"/success.html?gateway=vnpay&orderId={paymentResult.PaymentId}&vnp_TransactionNo={paymentResult.VnpayTransactionId}");
        }
        catch (VnpayException ex)
        {
            logger.LogWarning("VnPay Return failed: {Message}", ex.Message);
            return Redirect($"/error.html?gateway=vnpay&code={ex.PaymentResponseCode}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in VnPay Return");
            return Redirect("/error.html?gateway=vnpay");
        }
    }

    /// <summary>
    /// Creates a SePay payment checkout form.
    /// </summary>
    [HttpPost("sepay")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult CreateSepayPayment([FromBody] PaymentIntegration.Models.PaymentRequest request)
    {
        var html = sepayService.CreateCheckoutHtml(request);
        return Content(html, "text/html");
    }

    /// <summary>
    /// Handles SePay Webhook notifications.
    /// </summary>
    [HttpPost("sepay-webhook")]
    public IActionResult SepayWebhook()
    {
        logger.LogInformation("SePay Webhook received");
        return Ok(new { message = "Webhook received" });
    }

    /// <summary>
    /// Creates a PayOS payment link.
    /// </summary>
    [HttpPost("payos")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<string>>> CreatePayosPayment([FromBody] PaymentIntegration.Models.PaymentRequest request)
    {
        var url = await payosService.CreatePaymentUrl(request.OrderId, request.Amount, request.Description);
        return HandleResult(ApiResponse<string>.SuccessResult(url));
    }

    /// <summary>
    /// Handles PayOS Webhook notifications.
    /// </summary>
    [HttpPost("payos-webhook")]
    public async Task<IActionResult> PayosWebhook([FromBody] PayOS.Models.Webhooks.Webhook webhookData)
    {
        try
        {
            var verifiedData = await payosService.VerifyWebhook(webhookData);
            logger.LogInformation("PayOS Webhook verified for Order: {OrderCode}", verifiedData.OrderCode);
            return Ok(new { message = "Webhook received" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "PayOS Webhook verification failed");
            return BadRequest();
        }
    }
}
