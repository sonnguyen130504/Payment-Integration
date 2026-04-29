using System.Diagnostics;

namespace PaymentIntegration.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public string? TraceId { get; set; }

    public ApiResponse() 
    {
        TraceId = Activity.Current?.Id ?? string.Empty;
    }

    public ApiResponse(int statusCode, string? message = null, T? data = default)
    {
        StatusCode = statusCode;
        Success = statusCode >= 200 && statusCode < 300;
        Message = message;
        Data = data;
        TraceId = Activity.Current?.Id ?? string.Empty;
    }

    public static ApiResponse<T> SuccessResult(T data, string? message = "Success") 
        => new(200, message, data);

    public static ApiResponse<T> ErrorResult(int statusCode, string message) 
        => new(statusCode, message, default);
}

public class ApiResponse : ApiResponse<object>
{
    public ApiResponse(int statusCode, string? message = null) 
        : base(statusCode, message, default) { }
    
    public static ApiResponse SuccessResult(string? message = "Success") 
        => new(200, message);

    public new static ApiResponse ErrorResult(int statusCode, string message) 
        => new(statusCode, message);
}
