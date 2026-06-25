using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace DownloadScheduler.Options;

public abstract class DownloadOptionsBase
{
    [Required]
    [ValidateObjectMembers]
    public ScheduleOptions Schedule { get; init; } = new();

    public bool DownloadToArchive { get; init; }

    [Required]
    [Range(1, 365, ErrorMessage = "LookbackDays must be between 1 and 365.")]
    public int LookbackDays { get; init; }
}
