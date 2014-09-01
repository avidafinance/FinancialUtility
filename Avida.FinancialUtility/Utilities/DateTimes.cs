using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Avida.FinancialUtility.Utilities
{
    public static class DateTimes
    {
        public static DateTime? ParseDateExact(string date, string dateFormat)
        {
            DateTime d;
            if (DateTime.TryParseExact(date, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                return d;
            return null;
        }

        public static DateTime Min(params DateTime[] dates)
        {
            return dates.Min();
        }

        public static DateTime Max(params DateTime[] dates)
        {
            return dates.Max();
        }
    }
}