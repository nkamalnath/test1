using DownloadScheduler.BackgroundServices.Alerts;
using DownloadScheduler.BackgroundServices.Employee;
using DownloadScheduler.BackgroundServices.ErrorReports;
using DownloadScheduler.Extensions;
using DownloadScheduler.Options;

var builder = Host.CreateApplicationBuilder(args);

// ── HttpClient ────────────────────────────────────────────────────────────────
builder.Services.AddHttpClient("InternalApi", client =>
{
    var baseUrl = builder.Configuration["InternalApi:BaseUrl"]
        ?? throw new InvalidOperationException("InternalApi:BaseUrl is required in configuration.");

    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromMinutes(5);
});

// ── Options (validated at startup) ───────────────────────────────────────────
builder.Services.AddDownloadOptions<DownloadErrorReportsOptions>(DownloadErrorReportsOptions.SectionName);
builder.Services.AddDownloadOptions<DownloadAlertsOptions>(DownloadAlertsOptions.SectionName);
builder.Services.AddDownloadOptions<DownloadEmployeeOptions>(DownloadEmployeeOptions.SectionName);

// ── Background Services ───────────────────────────────────────────────────────
builder.Services.AddHostedService<ErrorReportsDownloadService>();
builder.Services.AddHostedService<AlertsDownloadService>();
builder.Services.AddHostedService<EmployeeDownloadService>();

var host = builder.Build();
host.Run();
