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

        //public static DateTime EndOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        //{
        //    DateTime thisWeekStart = dt.AddDays(-(int)dt.DayOfWeek);
        //    return thisWeekStart;
        //}
    }
}
