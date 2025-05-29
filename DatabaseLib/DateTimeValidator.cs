using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace Database
{
    internal static class DateTimeValidator
    {
        public static bool Validate<T>(List<T> list, Action<T, string> error) where T : IHasDateRange
        {
            for (int i = 0; i < list.Count() - 1; i++)
            {
                var current = list[i];
                var next = list[i + 1];

                if (current.EndDate().HasValue && current.EndDate().Value != next.StartDate())
                {
                    error(current, $"has period mismatch: { current.EndDate().Value} != { next.StartDate()}");
                    return false;
                }
                if (!current.EndDate().HasValue)
                {
                    error(current, $"has period mismatch: != {next.StartDate()}");
                    return false;
                }
            }
            return true;
        }
    }
}
