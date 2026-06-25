using DownloadScheduler.BackgroundServices.Base;
using DownloadScheduler.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DownloadScheduler.BackgroundServices.Alerts;

public class DownloadAlertsRequest : DownloadRequestBase
{
    // Add Alerts-specific properties here if needed in future
}

public class AlertsDownloadService
    : DownloadBackgroundServiceBase<DownloadAlertsOptions, DownloadAlertsRequest>
{
    public AlertsDownloadService(
        IOptions<DownloadAlertsOptions> options,
        IHttpClientFactory httpClientFactory,
        ILogger<AlertsDownloadService> logger)
        : base(options, httpClientFactory, logger) { }

    protected override DownloadAlertsRequest BuildRequest(DownloadAlertsOptions options) =>
        new()
        {
            LookbackDays = options.LookbackDays,
            DownloadToArchive = options.DownloadToArchive,
            ReportType = ReportType.Alert
        };
}
