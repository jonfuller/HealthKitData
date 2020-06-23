using System;
using System.Globalization;
using HealthKit;
using HealthKitData.Core;

namespace HealthKitData.iOS
{
    public static class RecordParser
    {
        public static Func<HKQuantitySample, Record> ParseWithUnit(HKUnit unit) => sample => ParseRecord(sample, unit);

        public static Record ParseRecord(HKQuantitySample sample, HKUnit unit)
        {
            var startDate = sample.StartDate.ToInstant();
            var endDate = sample.EndDate.ToInstant();

            return new Record
            {
                Type = sample.SampleType.ToString(),
                EndDate = endDate,
                StartDate = startDate,
                DateRange = new InstantRange(startDate, endDate),
                Value = sample.Quantity.GetDoubleValue(unit).ToString(CultureInfo.InvariantCulture),
                Unit = unit.UnitString,
                SourceName = sample.Source.Name
            };
        }
    }
}