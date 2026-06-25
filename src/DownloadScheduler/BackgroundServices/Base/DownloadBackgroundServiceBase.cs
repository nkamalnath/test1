using Cronos;
using DownloadScheduler.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DownloadScheduler.BackgroundServices.Base;

public abstract class DownloadBackgroundServiceBase<TOptions, TRequest> : BackgroundService
    where TOptions : DownloadOptionsBase
    where TRequest : DownloadRequestBase
{
    private readonly TOptions _options;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger _logger;

    private const string Endpoint = "/api/download";

    protected DownloadBackgroundServiceBase(
        IOptions<TOptions> options,
        IHttpClientFactory httpClientFactory,
        ILogger logger)
    {
        _options = options.Value;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    /// <summary>
    /// Maps the strongly-typed options to the POST request body for this download type.
    /// </summary>
    protected abstract TRequest BuildRequest(TOptions options);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Schedule.Enabled)
        {
            _logger.LogInformation("{Service} is disabled. Skipping.", GetType().Name);
            return;
        }

        var cronExpression = CronExpression.Parse(_options.Schedule.CronExpression);
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_options.Schedule.TimeZone);

        _logger.LogInformation("{Service} started. Cron: {Cron} | TimeZone: {TimeZone}",
            GetType().Name, _options.Schedule.CronExpression, _options.Schedule.TimeZone);

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTimeOffset.UtcNow;
            var next = cronExpression.GetNextOccurrence(now, timeZone);

            if (next is null)
            {
                _logger.LogWarning("{Service} cron expression has no future occurrences. Stopping.",
                    GetType().Name);
                break;
            }

            var delay = next.Value - now;
            _logger.LogInformation("{Service} next run scheduled at {Next} (in {Delay:hh\\:mm\\:ss}).",
                GetType().Name, next.Value.ToLocalTime(), delay);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            if (stoppingToken.IsCancellationRequested) break;

            await TriggerAsync(stoppingToken);
        }

        _logger.LogInformation("{Service} stopped.", GetType().Name);
    }

    private async Task TriggerAsync(CancellationToken stoppingToken)
    {
        try
        {
            var request = BuildRequest(_options);
            var client = _httpClientFactory.CreateClient("InternalApi");

            _logger.LogInformation(
                "{Service} posting to {Endpoint} — LookbackDays: {LookbackDays}, DownloadToArchive: {Archive}",
                GetType().Name, Endpoint, request.LookbackDays, request.DownloadToArchive);

            var response = await client.PostAsJsonAsync(Endpoint, request, stoppingToken);
            response.EnsureSuccessStatusCode();

            _logger.LogInformation("{Service} completed successfully.", GetType().Name);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "{Service} HTTP request failed. StatusCode: {StatusCode}",
                GetType().Name, ex.StatusCode);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "{Service} encountered an unexpected error.", GetType().Name);
        }
    }
}
