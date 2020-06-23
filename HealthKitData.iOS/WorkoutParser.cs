using HealthKit;
using HealthKitData.Core;
using LanguageExt;
using UnitsNet;
using static LanguageExt.Prelude;

namespace HealthKitData.iOS
{
    public static class WorkoutParser
    {
        public static Workout ParseWorkout(HKWorkout workout)
        {
            var startDate = workout.StartDate.ToInstant();
            var endDate = workout.EndDate.ToInstant();

            return new Workout
            {
                WorkoutType = workout.WorkoutActivityType.ToString(),
                SourceName = workout.Source.Name,
                EndDate = endDate,
                StartDate = startDate,
                Duration = Duration.FromMinutes(workout.Duration),
                Distance = workout.TotalDistance.Apply(Optional).Match(
                    Some: d => Length.FromMeters(workout.TotalDistance.GetDoubleValue(HKUnit.Meter)),
                    None: () => Length.Zero),
                Energy = workout.TotalEnergyBurned.Apply(Optional).Match(
                    Some: e => Energy.FromCalories(workout.TotalEnergyBurned.GetDoubleValue(HKUnit.Calorie)),
                    None: () => Energy.Zero),
                Device = workout.Device?.Name ?? "<no device>",
            };
        }
    }
}