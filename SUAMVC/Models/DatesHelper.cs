using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualBasic;

namespace SUAMVC.Models
{
    public class DatesHelper
    {
        public static long DateDiffInYears(DateTime dt1, DateTime dt2)
        {
            return DateAndTime.DateDiff(DateInterval.Year,
                dt1, dt2, FirstDayOfWeek.System, FirstWeekOfYear.System);
        }

        public static long DateDiffInMonths(DateTime dt1, DateTime dt2)
        {
            return DateAndTime.DateDiff(DateInterval.Month,
                dt1, dt2, FirstDayOfWeek.System, FirstWeekOfYear.System);
        }
    }
}