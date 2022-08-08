using System;
using System.Collections.Generic;
using System.Text;

namespace SOCE.Library.UI
{
    public static class DateTimeExt
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            DateTime thisWeekStart = dt.AddDays(-(int)dt.DayOfWeek);
            return thisWeekStart;
        }

        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            DateTime thisWeekStart = dt.AddDays(-(int)dt.DayOfWeek);
            return thisWeekStart;
        }
    }
}
