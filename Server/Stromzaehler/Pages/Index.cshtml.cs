using System;
using System.Collections.Generic;
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

            LastBlinks = thisYear.GroupBy(b => b.Source).ToDictionary(g => g.Key, g => g.First());

            var powerBlinks = thisYear.Where(b => b.Source == Source.Power).ToArray();
            AveragePeriod = (powerBlinks[0].Timestamp - powerBlinks[n - 1].Timestamp);
            var power = (powerBlinks[0].Value - powerBlinks[n - 1].Value) / AveragePeriod.TotalHours;
            CurrentPowerConsumption = power.ToString("0.0");

            PeriodConsumption = thisYear.GroupBy(b => b.Source)
                .Select(g =>
                {
                    var @default = g.Last();
                    var firstOfYear = g.FirstOrDefault(b => b.Timestamp.Year == now.Year - 1) ?? @default;
                    var firstOfMonth = g.FirstOrDefault(b => b.Timestamp.Month != now.Month) ?? @default;
                    var firstOfWeek = g.FirstOrDefault(b => b.Timestamp < now.StartOfWeek()) ?? @default;
                    var lastPoint = g.First();
                    return (lastPoint.Source, YearToDate: (lastPoint.Value - firstOfYear.Value),
                        MonthToDate: (lastPoint.Value - firstOfMonth.Value),
                        WeekToDate: (lastPoint.Value - firstOfWeek.Value));
                })
                .ToDictionary(i => i.Source);

            DailyAverage = thisYear.GroupBy(b => b.Timestamp.Date)
                .Select(g => g.First().Value - g.Last().Value)
                .Average();

            WeekDayAverages = thisYear.GroupBy(b => b.Timestamp.DayOfWeek)
                .Select(g => (g.Key, (double)g.Count() / g.GroupBy(c => c.Timestamp.Date).Count()))
                .ToArray();
        }

        public string BlinkCount { get; }
        public string LastBlinkCount { get; }
        public IDictionary<Source, Blink> LastBlinks { get; }
        public string CurrentPowerConsumption { get; }
        public Dictionary<Source, (Source Source, int YearToDate, int MonthToDate, int WeekToDate)> PeriodConsumption { get; }
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
