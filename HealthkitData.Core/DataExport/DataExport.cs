using System.Collections.Generic;
using System.IO;
using System.Linq;
using HealthKitData.Core.Excel;
using OfficeOpenXml;

namespace HealthKitData.Core.DataExport
{
    public static class DataExport
    {
        public static byte[] CreateExcelReport(byte[] exportZip, Excel.Settings.Settings settings, IEnumerable<ExcelWorksheet> customSheets)
        {
            using (var inputStream = new MemoryStream(exportZip))
            using (var outputStream = new MemoryStream())
            using (var excelFile = new ExcelPackage())
            {
                var loader = ZipUtilities.ReadArchive(
                        inputStream,
                        entry => entry.FullName == "apple_health_export/export.xml",
                        entry => new XmlReaderExportLoader(entry.Open()))
                   .FirstOrDefault();

                ExcelReport.BuildReport(loader.Records, loader.Workouts, excelFile.Workbook, settings, customSheets);

                excelFile.SaveAs(outputStream);

                return outputStream.ToArray();
            }
        }
    }
}