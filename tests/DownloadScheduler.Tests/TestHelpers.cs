using DownloadScheduler.Options;
using Microsoft.Extensions.Options;

namespace DownloadScheduler.Tests;

internal static class TestHelpers
{
    public static ScheduleOptions EnabledSchedule(string cron = "0 6 * * *") => new()
    {
        CronExpression = cron,
        TimeZone = "UTC",
        Enabled = true
    };

    public static ScheduleOptions DisabledSchedule() => new()
    {
        CronExpression = "0 6 * * *",
        TimeZone = "UTC",
        Enabled = false
    };

    public static IOptions<T> CreateOptions<T>(T value) where T : class
        => Microsoft.Extensions.Options.Options.Create(value);
}
