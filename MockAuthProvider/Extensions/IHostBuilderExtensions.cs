using NLog.Web;

namespace MockAuthProvider.Extensions
{
    public static class IHostBuilderExtensions
    {
        public static IHostBuilder RegisterNLog(this IHostBuilder hostBuilder)
            => hostBuilder.UseNLog(new NLogAspNetCoreOptions
            { 
                RemoveLoggerFactoryFilter = false,
                LoggingConfigurationSectionName = "NLog"
            });
    }
}
