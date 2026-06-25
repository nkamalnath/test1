namespace DownloadScheduler.BackgroundServices.Base;

public enum ReportType
{
    Alert,
    Employee,
    Error
}

public abstract class DownloadRequestBase
{
    public int LookbackDays { get; init; }
    public bool DownloadToArchive { get; init; }
    public ReportType ReportType { get; init; }
}
