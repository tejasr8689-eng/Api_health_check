using Api_health_check.Data;
using Api_health_check.Services;
using Api_health_check.Email;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Web.Administration;



var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<HealthCheckContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HealthCheckerDb")));
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<AuditLogService>();
builder.Services.AddSingleton<RetryPolicyService>();
builder.Services.AddSingleton<IisRecoveryService>();
builder.Services.AddSingleton<WebhookRecoveryService>();
builder.Services.AddScoped<RecoveryOrchestratorService>();

// HttpClient for background checks
builder.Services.AddHttpClient();

// Background worker that checks URLs
builder.Services.AddHostedService<UrlHealthCheckService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

using (var server = new ServerManager())
{
    foreach (var pool in server.ApplicationPools)
    {
        Console.WriteLine(pool.Name);
    }
}

app.Run();
