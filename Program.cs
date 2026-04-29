using PaymentIntegration.Models;
using PaymentIntegration.Services;
using PaymentIntegration.Interfaces;
using PaymentIntegration.Middlewares;
using Scalar.AspNetCore;
using VNPAY;
using VNPAY.Extensions;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

// Configure Payment Settings
builder.Services.Configure<VnpaySettings>(builder.Configuration.GetSection("PaymentSettings:Vnpay"));
builder.Services.Configure<SepaySettings>(builder.Configuration.GetSection("PaymentSettings:Sepay"));
builder.Services.Configure<PayosSettings>(builder.Configuration.GetSection("PaymentSettings:Payos"));

// Register VNPAY.NET SDK
builder.Services.AddVnpayClient(config =>
{
    var vnpaySection = builder.Configuration.GetSection("PaymentSettings:Vnpay");
    config.TmnCode = vnpaySection["TmnCode"]!;
    config.HashSecret = vnpaySection["HashSecret"]!;
    config.CallbackUrl = vnpaySection["CallbackUrl"]!;
    config.BaseUrl = vnpaySection["BaseUrl"]!;
    config.Version = vnpaySection["Version"] ?? "2.1.0";
});

// Register Payment Services (TicketFlow Style)
builder.Services.AddScoped<IPaymentService, VnpayService>();
builder.Services.AddScoped<IPaymentService, SepayService>();
builder.Services.AddScoped<IPaymentService, PayosService>();

// Also register concrete types for direct injection in controller
builder.Services.AddScoped<VnpayService>();
builder.Services.AddScoped<SepayService>();
builder.Services.AddScoped<PayosService>();

// Enable CORS for UI
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/index.html"));
app.MapControllers();

app.Run();
