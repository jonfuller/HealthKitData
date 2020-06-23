using Foundation;
using NodaTime;

namespace HealthKitData.iOS
{
    public static class NodaTimeExtensions
    {
        public static Instant ToInstant(this NSDate target)
        {
            return Instant.FromUnixTimeSeconds((long) target.SecondsSince1970);
        }
    }
}