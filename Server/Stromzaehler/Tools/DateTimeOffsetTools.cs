using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stromzaehler.Tools
{
    public static class DateTimeOffsetTools
    {
        public static DateTimeOffset  StartOfWeek(this DateTimeOffset date)
        {
            var day = (int)date.DayOfWeek;
            var shift = day - 1;
            if (shift < 0) shift += 7;
            var result = date.AddDays(-shift);
            return new DateTimeOffset(result.Year, result.Month, result.Day, 0, 0, 0, result.Offset);
        }
    }
}
