using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HealthKitData.Core.Excel.Sheets;
using NodaTime;
using OfficeOpenXml;

namespace HealthKitData.Core.Excel
{
    public static class ExcelReport
    {
        public static void BuildReport(IList<Record> records, IList<Workout> workouts, ExcelWorkbook workbook, Settings.Settings settings, DateTimeZone zone, IEnumerable<ExcelWorksheet> customSheets)
        {
            BuilderFactory.GetBuilders(settings, zone, records, workouts)
                .Select(b => new { b.sheetName, b.builder })
                .AsParallel().AsOrdered()
                .Select(b => new { b.sheetName, data = GetData(b.builder) })
                .AsSequential()
                .ToList().ForEach(s =>
                {
                    var sheet = workbook.Worksheets.Add(s.sheetName);
                    var wroteData = WriteData(sheet, s.data);

                    if (settings.OmitEmptySheets && !wroteData)
                    {
                        workbook.Worksheets.Delete(sheet);
                    }
                });

            workbook.PlaceCustomSheets(
                settings.CustomSheetsPlacement,
                customSheets,
                SheetNames.Summary);
        }

        private static object GetData(object builder)
        {
            var builderTypes = builder
                .GetType()
                .GetInterfaces()
                .Where(t => t.IsGenericType)
                .Single(t => t.GetGenericTypeDefinition() == typeof(IRawSheetBuilder<>))
                .GetGenericArguments();

            var openGetSheet = typeof(ExcelReport).GetMethod(nameof(GetRawSheetTyped), BindingFlags.Static | BindingFlags.NonPublic);
            var closedGetSheet = openGetSheet.MakeGenericMethod(builderTypes);

            return closedGetSheet.Invoke(null, new[] {builder});
        }

        private static bool WriteData(ExcelWorksheet sheet, object data)
        {
            var builderTypes = data
                .GetType()
                .GetGenericArguments();

            var openWriteSheet = typeof(ExcelReport).GetMethod(nameof(WriteSheetTyped), BindingFlags.Static | BindingFlags.NonPublic);
            var closedWriteSheet = openWriteSheet.MakeGenericMethod(builderTypes);

            return (bool)closedWriteSheet.Invoke(null, new[] { data, sheet });
        }

        private static Dataset<T> GetRawSheetTyped<T>(IRawSheetBuilder<T> builder)
        {
            return builder.BuildRawSheet();
        }

        private static bool WriteSheetTyped<T>(Dataset<T> sheetData, ExcelWorksheet sheet)
        {
            if (sheetData.Any())
            {
                sheet.WriteData(sheetData);
                return true;
            }

            return false;
        }
    }
}
