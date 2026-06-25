using DownloadScheduler.BackgroundServices.Alerts;
using DownloadScheduler.BackgroundServices.Employee;
using DownloadScheduler.BackgroundServices.ErrorReports;
using DownloadScheduler.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DownloadScheduler.Tests.BackgroundServices;

// ── Testable subclasses to expose BuildRequest ────────────────────────────────

internal class TestableErrorReportsService(
    IOptions<DownloadErrorReportsOptions> options,
    IHttpClientFactory factory)
    : ErrorReportsDownloadService(options, factory, NullLogger<ErrorReportsDownloadService>.Instance)
{
    public DownloadErrorReportsRequest ExposeBuildRequest(DownloadErrorReportsOptions o) => BuildRequest(o);
}

internal class TestableAlertsService(
    IOptions<DownloadAlertsOptions> options,
    IHttpClientFactory factory)
    : AlertsDownloadService(options, factory, NullLogger<AlertsDownloadService>.Instance)
{
    public DownloadAlertsRequest ExposeBuildRequest(DownloadAlertsOptions o) => BuildRequest(o);
}

internal class TestableEmployeeService(
    IOptions<DownloadEmployeeOptions> options,
    IHttpClientFactory factory)
    : EmployeeDownloadService(options, factory, NullLogger<EmployeeDownloadService>.Instance)
{
    public DownloadEmployeeRequest ExposeBuildRequest(DownloadEmployeeOptions o) => BuildRequest(o);
}

// ── BuildRequest mapping tests ────────────────────────────────────────────────

public class ErrorReportsDownloadServiceTests
{
    private static DownloadErrorReportsOptions DefaultOptions(bool enabled = true) => new()
    {
        LookbackDays = 30,
        DownloadToArchive = true,
        Schedule = TestHelpers.EnabledSchedule()
    };

    private static TestableErrorReportsService CreateService(DownloadErrorReportsOptions opts) =>
        new(TestHelpers.CreateOptions(opts), Mock.Of<IHttpClientFactory>());

    [Fact]
    public void BuildRequest_MapsLookbackDays_Correctly()
    {
        var options = DefaultOptions();
        var service = CreateService(options);

        var request = service.ExposeBuildRequest(options);

        Assert.Equal(30, request.LookbackDays);
    }

    [Fact]
    public void BuildRequest_MapsDownloadToArchive_Correctly()
    {
        var options = DefaultOptions();
        var service = CreateService(options);

        var request = service.ExposeBuildRequest(options);

        Assert.True(request.DownloadToArchive);
    }

    [Fact]
    public async Task ExecuteAsync_WhenDisabled_DoesNotCallHttpClient()
    {
        var mockFactory = new Mock<IHttpClientFactory>();
        var options = new DownloadErrorReportsOptions
        {
            LookbackDays = 30,
            DownloadToArchive = false,
            Schedule = TestHelpers.DisabledSchedule()
        };

        var service = new ErrorReportsDownloadService(
            TestHelpers.CreateOptions(options),
            mockFactory.Object,
            NullLogger<ErrorReportsDownloadService>.Instance);

        var cts = new CancellationTokenSource();
        await service.StartAsync(cts.Token);
        await service.StopAsync(cts.Token);

        mockFactory.Verify(f => f.CreateClient(It.IsAny<string>()), Times.Never);
    }
}

public class AlertsDownloadServiceTests
{
    private static DownloadAlertsOptions DefaultOptions() => new()
    {
        LookbackDays = 7,
        DownloadToArchive = false,
        Schedule = TestHelpers.EnabledSchedule()
    };

    private static TestableAlertsService CreateService(DownloadAlertsOptions opts) =>
        new(TestHelpers.CreateOptions(opts), Mock.Of<IHttpClientFactory>());

    [Fact]
    public void BuildRequest_MapsLookbackDays_Correctly()
    {
        var options = DefaultOptions();
        var request = CreateService(options).ExposeBuildRequest(options);

        Assert.Equal(7, request.LookbackDays);
    }

    [Fact]
    public void BuildRequest_MapsDownloadToArchive_Correctly()
    {
        var options = DefaultOptions();
        var request = CreateService(options).ExposeBuildRequest(options);

        Assert.False(request.DownloadToArchive);
    }
}

public class EmployeeDownloadServiceTests
{
    private static DownloadEmployeeOptions DefaultOptions() => new()
    {
        LookbackDays = 90,
        DownloadToArchive = true,
        Schedule = TestHelpers.EnabledSchedule()
    };

    private static TestableEmployeeService CreateService(DownloadEmployeeOptions opts) =>
        new(TestHelpers.CreateOptions(opts), Mock.Of<IHttpClientFactory>());

    [Fact]
    public void BuildRequest_MapsLookbackDays_Correctly()
    {
        var options = DefaultOptions();
        var request = CreateService(options).ExposeBuildRequest(options);

        Assert.Equal(90, request.LookbackDays);
    }

    [Fact]
    public void BuildRequest_MapsDownloadToArchive_Correctly()
    {
        var options = DefaultOptions();
        var request = CreateService(options).ExposeBuildRequest(options);

        Assert.True(request.DownloadToArchive);
    }

    [Fact]
    public async Task ExecuteAsync_WhenDisabled_DoesNotCallHttpClient()
    {
        var mockFactory = new Mock<IHttpClientFactory>();
        var options = new DownloadEmployeeOptions
        {
            LookbackDays = 90,
            DownloadToArchive = true,
            Schedule = TestHelpers.DisabledSchedule()
        };

        var service = new EmployeeDownloadService(
            TestHelpers.CreateOptions(options),
            mockFactory.Object,
            NullLogger<EmployeeDownloadService>.Instance);

        var cts = new CancellationTokenSource();
        await service.StartAsync(cts.Token);
        await service.StopAsync(cts.Token);

        mockFactory.Verify(f => f.CreateClient(It.IsAny<string>()), Times.Never);
    }
}
