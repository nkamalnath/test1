using DownloadScheduler.Options;
using Microsoft.Extensions.DependencyInjection;

namespace DownloadScheduler.Extensions;

public static class OptionsRegistrationExtensions
{
    public static IServiceCollection AddDownloadOptions<T>(
        this IServiceCollection services,
        string sectionName)
        where T : DownloadOptionsBase
    {
        services
            .AddOptions<T>()
            .BindConfiguration(sectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}
