using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthKitData.Core.Excel.Sheets
{
    public static class SheetExtensions
    {
        public static Column<TKey> MakeColumn<TKey, TVal>(this IEnumerable<Tuple<TKey, TVal>> data, string header = null, string range = null)
        {
            return data.Aggregate(
                new Column<TKey> { Header = header, RangeName = range },
                (col, r) => { col.Add(r.Item1, r.Item2); return col; });
        }
    }
}
