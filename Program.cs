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

// Configure Payment Settings (TicketFlow Style)
builder.Services.Configure<VnpaySettings>(builder.Configuration.GetSection(VnpaySettings.SectionName));
builder.Services.Configure<SepaySettings>(builder.Configuration.GetSection(SepaySettings.SectionName));
builder.Services.Configure<PayosSettings>(builder.Configuration.GetSection(PayosSettings.SectionName));

// Register VNPAY.NET SDK (TicketFlow Style)
builder.Services.AddVnpayClient(config =>
{
    var vnpaySection = builder.Configuration.GetSection(VnpaySettings.SectionName);
    config.TmnCode = vnpaySection["TmnCode"]!;
    config.HashSecret = vnpaySection["HashSecret"]!;
    config.CallbackUrl = vnpaySection["CallbackUrl"]!;
    config.BaseUrl = vnpaySection["BaseUrl"]!;
    config.Version = vnpaySection["Version"] ?? "2.1.0";
    config.OrderType = vnpaySection["OrderType"] ?? "other";
});

// Register Payment Services
builder.Services.AddScoped<IPaymentService, VnpayService>();
builder.Services.AddScoped<IPaymentService, SepayService>();
builder.Services.AddScoped<IPaymentService, PayosService>();

// Concrete registrations
builder.Services.AddScoped<VnpayService>();
builder.Services.AddScoped<SepayService>();
builder.Services.AddScoped<PayosService>();

// Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

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
