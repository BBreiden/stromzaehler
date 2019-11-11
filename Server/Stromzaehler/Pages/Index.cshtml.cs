using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stromzaehler.Models;
using Stromzaehler.Tools;

namespace Stromzaehler.Pages
{

    // Todo:
    // - YearToDate, Week Month
    // - Averages Daily, Weekly, Monthly
    // - Intraday Histogram (category of days => workday, weekend, holiday)
    // - "typical day" histogram vs today
    public class IndexModel : PageModel
    {
        public IndexModel(BlinkDataContext database)
        {
            BlinkCount = database.Blinks.Count().ToString("##,#");
            var now = DateTimeOffset.Now;
            var n = 10;
            var thisYear = database.Blinks.AsNoTracking()
                .Where(b => b.Timestamp.Year == now.Year)
                .OrderByDescending(b => b.Timestamp)
                .ToArray();

            LastBlinks = thisYear[0];
            LastBlinkCount = LastBlinks.Value.ToString("##,#");

            AveragePeriod = (thisYear[0].Timestamp - thisYear[n - 1].Timestamp);
            var power = (thisYear[0].Value - thisYear[n - 1].Value) / AveragePeriod.TotalHours;
            CurrentConsumption = power.ToString("0.0");

            var firstBlink = thisYear[thisYear.Length - 1];
            YearToDate = (thisYear[0].Value - firstBlink.Value) / 1000;
            MonthToDate = (thisYear[0].Value
                - (thisYear.FirstOrDefault(b => b.Timestamp.Month != now.Month) ?? firstBlink).Value) / 1000;
            WeekToDate = (thisYear[0].Value - thisYear.First(b => b.Timestamp < now.StartOfWeek()).Value) / 1000;

            DailyAverage = thisYear.GroupBy(b => b.Timestamp.Date)
                .Select(g => g.First().Value - g.Last().Value)
                .Average();

            WeekDayAverages = thisYear.GroupBy(b => b.Timestamp.DayOfWeek)
                .Select(g => (g.Key, (double)g.Count() / g.GroupBy(c => c.Timestamp.Date).Count()))
                .ToArray();
        }

        public string BlinkCount { get; }
        public string LastBlinkCount { get; }
        public Blink LastBlinks { get; }
        public string CurrentConsumption { get; }
        public int YearToDate { get; }
        public int MonthToDate { get; }
        public int WeekToDate { get; }
        public double DailyAverage { get; }
        public (DayOfWeek Key, double)[] WeekDayAverages { get; }
        public TimeSpan AveragePeriod { get; }
        public string GetPeriod()
        {
            if (AveragePeriod.Hours > 0)
            {
                return $"{AveragePeriod.TotalHours.ToString("0.0")} hours";
            }
            else if (AveragePeriod.Minutes > 0)
            {
                return $"{AveragePeriod.TotalMinutes.ToString("0.0")} minutes";
            }
            return $"{AveragePeriod.TotalSeconds.ToString("0.0")} seconds";
        }

        public void OnGet()
        {

        }
    }
}
