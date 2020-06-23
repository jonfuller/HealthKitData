using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HealthKit;
using HealthKitData.Core;
using LanguageExt;
using LanguageExt.SomeHelp;
using NodaTime;
using Xamarin.Forms.Platform.iOS;

using static LanguageExt.Prelude;

namespace HealthKitData.iOS
{
    public static class HealthKitQueries
    {
        public static async Task<IEnumerable<Workout>> GetWorkouts(HKHealthStore store, DateInterval dates)
        {
            var workoutType = HKObjectType.GetWorkoutType();

            var workouts = await RunQuery(store, workoutType, dates, sample => WorkoutParser.ParseWorkout((HKWorkout)sample));

            return workouts.Match(
                Some: w => w,
                None: Enumerable.Empty<Workout>());
        }

        public static async Task<IEnumerable<Record>> GetHealthRecords(HKHealthStore store, DateInterval dates)
        {
            var query = Query(store, dates);

            var queries = new[]
            {
                (HKQuantityType.Create(HKQuantityTypeIdentifier.StepCount), HKUnit.Count),
                (HKQuantityType.Create(HKQuantityTypeIdentifier.BodyMass), HKUnit.Pound),
                (HKQuantityType.Create(HKQuantityTypeIdentifier.BodyFatPercentage), HKUnit.Percent),
                (HKQuantityType.Create(HKQuantityTypeIdentifier.RestingHeartRate), HKUnit.HertzUnit),
                (HKQuantityType.Create(HKQuantityTypeIdentifier.WalkingHeartRateAverage), HKUnit.HertzUnit),
                (HKQuantityType.Create(HKQuantityTypeIdentifier.VO2Max), HKUnit.Percent),
                (HKQuantityType.Create(HKQuantityTypeIdentifier.AppleStandTime), HKUnit.Minute),
                (HKQuantityType.Create(HKQuantityTypeIdentifier.FlightsClimbed), HKUnit.Count),
                (HKQuantityType.Create(HKQuantityTypeIdentifier.AppleExerciseTime), HKUnit.Minute),
                (HKQuantityType.Create(HKQuantityTypeIdentifier.BasalEnergyBurned), HKUnit.Calorie),
                (HKQuantityType.Create(HKQuantityTypeIdentifier.ActiveEnergyBurned), HKUnit.Calorie),
            }
            .Select(r => query(r.Item1, r.Item2));

            return (await Task.WhenAll(queries))
               .Select(x => x.Match(
                    Some: s => s,
                    None: Enumerable.Empty<Record>()))
               .Flatten();

            static Func<HKQuantityType, HKUnit, Task<Option<IEnumerable<Record>>>> Query(HKHealthStore store, DateInterval dates) =>
                (type, unit) => RunQuantitySampleQuery(store, type, dates, s => RecordParser.ParseRecord(s, unit));
        }

        private static Task<Option<IEnumerable<TOut>>> RunQuantitySampleQuery<TOut>(HKHealthStore store, HKQuantityType type, DateInterval dates, Func<HKQuantitySample, TOut> mapper)
        {
            return RunQuery(store, type, dates, sample => mapper((HKQuantitySample)sample));
        }

        private static Task<Option<IEnumerable<TOut>>> RunQuery<TOut>(HKHealthStore store, HKSampleType type, DateInterval dates, Func<HKSample, TOut> mapper)
        {
            var samplePredicate = HKQuery.GetPredicateForSamples(
                dates.Start.ToDateTimeUnspecified().ToNSDate(),
                dates.End.ToDateTimeUnspecified().ToNSDate(),
                HKQueryOptions.None);

            var theResults = new Option<IEnumerable<TOut>>();

            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

            var query = new HKSampleQuery(type, samplePredicate, HKSampleQuery.NoLimit, null, (_, results, err) =>
            {
                if (results == null)
                    theResults = None;
                else
                    theResults = results
                       .Select(mapper)
                       .ToSome();

                waitHandle.Set();
            });

            store.ExecuteQuery(query);

            return Task.Run(() =>
            {
                waitHandle.WaitOne(TimeSpan.FromSeconds(30));
                return theResults;
            });
        }
    }
}