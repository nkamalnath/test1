using System.ComponentModel.DataAnnotations;

namespace DownloadScheduler.Options;

public class ScheduleOptions
{
    [Required(AllowEmptyStrings = false, ErrorMessage = "CronExpression is required.")]
    public string CronExpression { get; init; } = string.Empty;

    [Required(AllowEmptyStrings = false, ErrorMessage = "TimeZone is required.")]
    public string TimeZone { get; init; } = string.Empty;

    public bool Enabled { get; init; }
}
