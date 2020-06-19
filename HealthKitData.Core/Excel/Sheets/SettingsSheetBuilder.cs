﻿using System.Linq;
using HealthKitData.Core.Excel.Settings;

namespace HealthKitData.Core.Excel.Sheets
{
    public class SettingsSheetBuilder : IRawSheetBuilder<unit>
    {
        private readonly Settings.Settings _settings;

        public SettingsSheetBuilder(Settings.Settings settings)
        {
            _settings = settings;
        }

        public Dataset<unit> BuildRawSheet()
        {
            var columns = _settings
                .Aggregate(new
                    {
                        name = new Column<unit> { Header = ColumnNames.Settings.Name() },
                        value = new Column<unit> { Header = ColumnNames.Settings.Value() },
                        defaultValue = new Column<unit> { Header = ColumnNames.Settings.DefaultValue() },
                        description = new Column<unit> { Header = ColumnNames.Settings.Description() },
                    },
                    (cols, s) =>
                    {
                        cols.name.Add(unit.v, s.Name);
                        cols.value.Add(unit.v, s.ExcelSerialization == SerializationBehavior.Nothing
                            ? s.Value
                            : s.Value.ToString());
                        cols.defaultValue.Add(unit.v, s.DefaultValue);
                        cols.description.Add(unit.v, s.Description);
                        return cols;
                    });

            return new Dataset<unit>(columns.name, columns.value, columns.defaultValue, columns.description);
        }
    }
}