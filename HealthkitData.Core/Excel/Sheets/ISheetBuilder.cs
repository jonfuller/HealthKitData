using System.Collections.Generic;
using NodaTime;

namespace HealthKitData.Core.Excel.Sheets
{
    public interface IRawSheetBuilder<TKey>
    {
        Dataset<TKey> BuildRawSheet();
    }
    public interface IMonthlySummaryBuilder<TKey>
    {
        IEnumerable<Column<TKey>> BuildSummaryForDateRange(IRange<ZonedDateTime> dateRange);
    }
    public interface ISummarySheetBuilder<TKey>
    {
        IEnumerable<Column<TKey>> BuildSummary();
    }
}
