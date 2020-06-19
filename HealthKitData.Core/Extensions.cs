using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HealthKitData.Core
{
    public static class Extensions
    {
        public static string SplitCamelCase(this string input)
        {
            return Regex.Replace(input, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled);
        }


        public static TV GetValue<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV))
        {
            return dict.TryGetValue(key, out var value) ? value : defaultValue;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> target)
        {
            return !target.Any();
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> target, T item)
        {
            return target.Concat(new[] { item });
        }

        public static double SafeParse(this string target, double valueIfParseFail)
        {
            var parsed = double.TryParse(target, out var result);
            return parsed ? result : valueIfParseFail;
        }
    }
}