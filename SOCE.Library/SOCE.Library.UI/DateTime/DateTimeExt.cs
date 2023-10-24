using System;
using System.Collections.Generic;
using System.Text;

namespace SOCE.Library.UI
{
    public static class DateTimeExt
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime GetNextWeekday(this DateTime start, DayOfWeek day)
        {
            // The (... + 7) % 7 ensures we end up with a value in the range [0, 6]
            int daysToAdd = ((int)day - (int)start.DayOfWeek + 7) % 7;
            return start.AddDays(daysToAdd);
        }

        //public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        //{
        //    DateTime thisWeekStart = dt.AddDays(-(int)dt.DayOfWeek);
        //    return thisWeekStart;
        //}
    }
}
