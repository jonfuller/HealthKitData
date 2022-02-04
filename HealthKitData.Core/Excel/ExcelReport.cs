using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HealthKitData.Core.Excel.Sheets;
using Microsoft.Extensions.Logging;
using NodaTime;
using OfficeOpenXml;

namespace HealthKitData.Core.Excel
{
    public static class ExcelReport
    {
        public static void BuildReport(IList<Record> records, IList<Workout> workouts, ExcelWorkbook workbook, Settings.Settings settings, DateTimeZone zone, IEnumerable<ExcelWorksheet> customSheets)
        {
            var logger = Logging.LoggerFactory.CreateLogger(typeof(ExcelReport));

            BuilderFactory.GetBuilders(settings, zone, records, workouts)
                .Select(b => new { b.sheetName, b.builder })
                .Select(b =>
                {
                    logger.LogDebug($"builder: {b.sheetName} -> {b.builder.GetType()}");
                    return b;
                })
                .ToList()
                .AsParallel().AsOrdered()
                .Select(b => new { b.sheetName, data = GetData(b.builder, logger) })
                .AsSequential()
                .ToList().ForEach(s =>
                {
                    var sheet = workbook.Worksheets.Add(s.sheetName);
                    var wroteData = WriteData(sheet, s.data, logger);

                    if (settings.OmitEmptySheets && !wroteData)
                    {
                        logger.LogDebug($"removing empty sheet {sheet.Name}");
                        workbook.Worksheets.Delete(sheet);
                    }
                });

            logger.LogDebug($"placing custom sheets: {settings.CustomSheetsPlacement}");
            workbook.PlaceCustomSheets(
                settings.CustomSheetsPlacement,
                customSheets,
                SheetNames.Summary);
        }

        private static object GetData(object builder, ILogger logger)
        {
            var builderTypes = builder
                .GetType()
                .GetInterfaces()
                .Where(t => t.IsGenericType)
                .Single(t => t.GetGenericTypeDefinition() == typeof(IRawSheetBuilder<>))
                .GetGenericArguments();

            var openGetSheet = typeof(ExcelReport).GetMethod(nameof(GetRawSheetTyped), BindingFlags.Static | BindingFlags.NonPublic);
            var closedGetSheet = openGetSheet.MakeGenericMethod(builderTypes);

            logger.LogDebug($"getting data for {builder.GetType()}");
            return closedGetSheet.Invoke(null, new[] {builder});
        }

        private static bool WriteData(ExcelWorksheet sheet, object data, ILogger logger)
        {
            var builderTypes = data
                .GetType()
                .GetGenericArguments();

            var openWriteSheet = typeof(ExcelReport).GetMethod(nameof(WriteSheetTyped), BindingFlags.Static | BindingFlags.NonPublic);
            var closedWriteSheet = openWriteSheet.MakeGenericMethod(builderTypes);

            logger.LogDebug($"writing data for {sheet.Name}");
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
