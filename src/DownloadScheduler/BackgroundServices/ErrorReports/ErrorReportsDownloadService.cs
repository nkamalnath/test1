using DownloadScheduler.BackgroundServices.Base;
using DownloadScheduler.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DownloadScheduler.BackgroundServices.ErrorReports;

public class DownloadErrorReportsRequest : DownloadRequestBase
{
    // Add ErrorReports-specific properties here if needed in future
}

public class ErrorReportsDownloadService
    : DownloadBackgroundServiceBase<DownloadErrorReportsOptions, DownloadErrorReportsRequest>
{
    public ErrorReportsDownloadService(
        IOptions<DownloadErrorReportsOptions> options,
        IHttpClientFactory httpClientFactory,
        ILogger<ErrorReportsDownloadService> logger)
        : base(options, httpClientFactory, logger) { }

    protected override DownloadErrorReportsRequest BuildRequest(DownloadErrorReportsOptions options) =>
        new()
        {
            LookbackDays = options.LookbackDays,
            DownloadToArchive = options.DownloadToArchive,
            ReportType = ReportType.Error
        };
}
