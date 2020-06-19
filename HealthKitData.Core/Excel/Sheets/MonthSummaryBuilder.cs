﻿using System;
using System.Collections.Generic;
using System.Linq;
using HealthKitData.Core.Excel.Sheets.Records;
using HealthKitData.Core.Excel.Sheets.Workouts;
using NodaTime;

namespace HealthKitData.Core.Excel.Sheets
{
    public class MonthSummaryBuilder : IRawSheetBuilder<LocalDate>
    {
        private readonly IEnumerable<Column<LocalDate>> _columns;
        private readonly List<LocalDate> _monthDays;

        public MonthSummaryBuilder(int targetYear, int targetMonth, DateTimeZone zone,
            StepBuilder stepBuilder,
            GeneralRecordsBuilder generalRecordsBuilder,
            HealthMarkersBuilder healthMarkersBuilder,
            NutritionBuilder nutritionBuilder,
            WorkoutBuilderFactory workoutBuilderFactory,
            DistanceCyclingBuilder distanceCyclingBuilder,
            MassBuilder massBuilder,
            BodyFatPercentageBuilder bodyFatBuilder)
        {
            _monthDays = Enumerable.Range(1, DateTime.DaysInMonth(targetYear, targetMonth))
                .Select(d => new LocalDate(targetYear, targetMonth, d))
                .ToList();

            var range = new DateRange(_monthDays.First().AtStartOfDayInZone(zone), _monthDays.Last().AtStartOfDayInZone(zone));

            _columns = Enumerable.Empty<Column<LocalDate>>()
                .Concat(stepBuilder.BuildSummaryForDateRange(range))
                .Concat(bodyFatBuilder.BuildSummaryForDateRange(range))
                .Concat(generalRecordsBuilder.BuildSummaryForDateRange(range))
                .Concat(healthMarkersBuilder.BuildSummaryForDateRange(range))
                .Concat(nutritionBuilder.BuildSummaryForDateRange(range))
                .Concat(massBuilder.BuildSummaryForDateRange(range))
                .Concat(distanceCyclingBuilder.BuildSummaryForDateRange(range))
                .Concat(workoutBuilderFactory.GetWorkoutBuilders().SelectMany(b => b.BuildSummaryForDateRange(range)))
                ;
        }

        public Dataset<LocalDate> BuildRawSheet()
        {
            return new Dataset<LocalDate>(
                new KeyColumn<LocalDate>(_monthDays){Header = ColumnNames.Date()},
                _columns.ToArray());
        }
    }
}