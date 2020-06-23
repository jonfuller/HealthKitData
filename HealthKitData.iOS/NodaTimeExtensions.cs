using System;
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

        public static NSDate ToNSDate(this LocalDate target)
        {
            var referenceDate = new DateTime(2001, 1, 1, 0, 0, 0);

            return NSDate.FromTimeIntervalSinceReferenceDate((target.ToDateTimeUnspecified() - referenceDate).TotalSeconds);
        }
    }
}