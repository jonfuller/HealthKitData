using NodaTime;
using UnitsNet;
using UNDuration = UnitsNet.Duration;

namespace HealthKitData.Core
{
    public class Workout
    {
        public string WorkoutType { get; set; }
        public string SourceName { get; set; }
        public Instant StartDate { get; set; }
        public Instant EndDate { get; set; }
        public UNDuration Duration { get; set; }
        public Length Distance { get; set; }
        public Energy Energy { get; set; }
        public string Device { get; set; }
    }
}
