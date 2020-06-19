using System.Collections.Generic;

namespace HealthKitData.Core.DataExport
{
    public interface IExportLoader
    {
        IList<Workout> Workouts { get; }
        IList<Record> Records { get; }
    }
}
