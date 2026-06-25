using System.ComponentModel.DataAnnotations;
using DownloadScheduler.Options;
using Xunit;

namespace DownloadScheduler.Tests.Options;

public class DownloadOptionsValidationTests
{
    private static List<ValidationResult> Validate(object obj)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(obj, null, null);
        Validator.TryValidateObject(obj, context, results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void DownloadErrorReportsOptions_ValidConfig_PassesValidation()
    {
        var options = new DownloadErrorReportsOptions
        {
            LookbackDays = 30,
            DownloadToArchive = true,
            Schedule = new ScheduleOptions
            {
                CronExpression = "0 6 * * *",
                TimeZone = "UTC",
                Enabled = true
            }
        };

        var results = Validate(options);
        Assert.Empty(results);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(366)]
    public void DownloadOptionsBase_InvalidLookbackDays_FailsValidation(int lookbackDays)
    {
        var options = new DownloadErrorReportsOptions
        {
            LookbackDays = lookbackDays,
            DownloadToArchive = false,
            Schedule = new ScheduleOptions
            {
                CronExpression = "0 6 * * *",
                TimeZone = "UTC",
                Enabled = true
            }
        };

        var results = Validate(options);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(DownloadOptionsBase.LookbackDays)));
    }

    [Fact]
    public void ScheduleOptions_MissingCronExpression_FailsValidation()
    {
        var options = new ScheduleOptions
        {
            CronExpression = "",
            TimeZone = "UTC",
            Enabled = true
        };

        var results = Validate(options);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(ScheduleOptions.CronExpression)));
    }

    [Fact]
    public void ScheduleOptions_MissingTimeZone_FailsValidation()
    {
        var options = new ScheduleOptions
        {
            CronExpression = "0 6 * * *",
            TimeZone = "",
            Enabled = true
        };

        var results = Validate(options);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(ScheduleOptions.TimeZone)));
    }
}
