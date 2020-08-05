# HealthKitData
HealthKit Data Manipulation for .NET

## Overview

### HealthKitData.Core

A .NET Standard library for data processing and report generation for data exports from Apple HealthKit.

![HealthKitData.Core](https://github.com/jonfuller/HealthKitData/workflows/HealthKitData.Core/badge.svg)
![Nuget](https://img.shields.io/nuget/v/HealthKitData.Core?style=flat)

### HealthKitData.iOS

A Xamarin.iOS library for querying data directly from HealthKit on the iOS mobile device.

![HealthKitData.iOS](https://github.com/jonfuller/HealthKitData/workflows/HealthKitData.iOS/badge.svg)
![Nuget](https://img.shields.io/nuget/v/HealthKitData.iOS?style=flat)

## Usage

### HealthKitData.Core

Example of loading a full data export from Apple Health and exporting it to Excel...

```
var loader = Using(new StreamReader("export.zip"), reader =>
    ZipUtilities.ReadArchive(
        reader.BaseStream,
        entry => entry.FullName == "apple_health_export/export.xml",
        entry => new XmlReaderExportLoader(entry.Open()))
    .FirstOrDefault());

using (var excelFile = new ExcelPackage())
{
    ExcelReport.BuildReport(loader.Records, loader.Workouts, excelFile.Workbook, Settings.Default, customSheets: Enumerable.Empty<ExcelWorksheet>());

    excelFile.SaveAs(new FileInfo(@"output.xlsx"));
}
```

### HealtKitData.iOS

Example of authorizing with `HealthKit` and then querying workouts.

```
var store = new HKHealthStore();

var dateRange = new DateInterval(
    start: LocalDate.FromDateTime(new DateTime(2019, 01, 01))),
    end: LocalDate.FromDateTime(new DateTime(2020, 01, 01)));

await store.RequestAuthorizationToShareAsync(
    typesToShare: new NSSet(),
    typesToRead: new NSSet(HKObjectType.GetWorkoutType()));

var workouts = await HealthKitQueries.GetWorkouts(store, dateRange);
```
