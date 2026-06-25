using DownloadScheduler.BackgroundServices.Base;
using DownloadScheduler.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DownloadScheduler.BackgroundServices.Employee;

public class DownloadEmployeeRequest : DownloadRequestBase
{
    // Add Employee-specific properties here if needed in future
    // e.g. public string? DepartmentFilter { get; init; }
}

public class EmployeeDownloadService
    : DownloadBackgroundServiceBase<DownloadEmployeeOptions, DownloadEmployeeRequest>
{
    public EmployeeDownloadService(
        IOptions<DownloadEmployeeOptions> options,
        IHttpClientFactory httpClientFactory,
        ILogger<EmployeeDownloadService> logger)
        : base(options, httpClientFactory, logger) { }

    protected override DownloadEmployeeRequest BuildRequest(DownloadEmployeeOptions options) =>
        new()
        {
            LookbackDays = options.LookbackDays,
            DownloadToArchive = options.DownloadToArchive,
            ReportType = ReportType.Employee
        };
}
