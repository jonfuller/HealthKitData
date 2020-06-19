﻿using System;
using UnitsNet;
using UnitsNet.Units;

namespace HealthKitData.Core.Excel.Settings
{
    public static class Coercion
    {
        public static object Coerce(object value, Type targetType)
        {
            if (targetType == typeof(int))
            {
                return CoerceInt32(value);
            }
            if (targetType == typeof(float))
            {
                return CoerceFloat(value);
            }
            if (targetType == typeof(bool))
            {
                return CoerceBool(value);
            }
            if (targetType == typeof(LengthUnit))
            {
                return CoerceLengthUnit(value);
            }
            if (targetType == typeof(DurationUnit))
            {
                return CoerceDuration(value);
            }
            if (targetType == typeof(MassUnit))
            {
                return CoerceMass(value);
            }
            if (targetType == typeof(EnergyUnit))
            {
                return CoerceEnergy(value);
            }
            if (targetType == typeof(CustomSheetsPlacement))
            {
                return CoerceEnum((string) value, CustomSheetsPlacement.Last);
            }

            return CoerceString(value);
        }

        private static TEnum CoerceEnum<TEnum>(string value, TEnum defaultValue) where TEnum : struct
        {
            return Enum.TryParse(value, out TEnum placement)
                ? placement
                : defaultValue;
        }

        private static DurationUnit CoerceDuration(object value)
        {
            try
            {
                return Duration.ParseUnit(value.ToString());
            }
            catch (Exception)
            {
                return Enum.TryParse(value.ToString(), true, out DurationUnit unit)
                    ? unit
                    : DurationUnit.Minute;
            }
        }

        private static MassUnit CoerceMass(object value)
        {
            try
            {
                return Mass.ParseUnit(value.ToString());
            }
            catch (Exception)
            {
                return Enum.TryParse(value.ToString(), true, out MassUnit unit)
                    ? unit
                    : MassUnit.Pound;
            }
        }

        private static EnergyUnit CoerceEnergy(object value)
        {
            try
            {
                return Energy.ParseUnit(value.ToString());
            }
            catch (Exception)
            {
                return Enum.TryParse(value.ToString(), true, out EnergyUnit unit)
                    ? unit
                    : EnergyUnit.Kilocalorie;
            }
        }

        private static LengthUnit CoerceLengthUnit(object value)
        {
            try
            {
                return Length.ParseUnit(value.ToString());
            }
            catch (Exception)
            {
                return Enum.TryParse(value.ToString(), true, out LengthUnit unit)
                    ? unit
                    : LengthUnit.Mile;
            }
        }

        private static int CoerceInt32(object value)
        {
            if (value is int i) return i;
            if (value is long l) return Convert.ToInt32(l);
            if (value is float f) return Convert.ToInt32(f);
            if (value is double m) return Convert.ToInt32(m);
            if (value is decimal d) return Convert.ToInt32(d);
            if (value is string s && (int.TryParse(s, out var val))) return val;

            return 0;
        }
        private static float CoerceFloat(object value)
        {
            if (value is float f) return f;
            if (value is int i) return Convert.ToSingle(i);
            if (value is long l) return Convert.ToSingle(l);
            if (value is double m) return Convert.ToSingle(m);
            if (value is decimal d) return Convert.ToSingle(d);
            if (value is string s && (float.TryParse(s, out var val))) return val;

            return 0;
        }
        private static bool CoerceBool(object value)
        {
            if (value is bool b) return b;
            if (value is string s) return s.Equals("true", StringComparison.CurrentCultureIgnoreCase);

            return false;
        }
        private static string CoerceString(object value)
        {
            return value.ToString();
        }
    }
}