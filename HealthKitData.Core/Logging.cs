using Microsoft.Extensions.Logging;

namespace HealthKitData.Core
{
    public static class Logging
    {
        public static ILoggerFactory LoggerFactory { get; private set; } = Microsoft.Extensions.Logging.LoggerFactory.Create(
            b => b.SetMinimumLevel(LogLevel.Information));

        public static void Configure(ILoggerFactory loggerFactory)
        {
            LoggerFactory = loggerFactory;
        }
    }
}