using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        public IndexModel(IBlinkData blinks, ILogger<IndexModel> log)
        {
            var w = Stopwatch.StartNew();
            log.LogInformation($"started @ {w.Elapsed.TotalMilliseconds}");
            BlinkCount = blinks.Count.ToString("##,#");
            var now = DateTimeOffset.Now;
            log.LogInformation($"loaded @ {w.Elapsed.TotalMilliseconds}");

            LastBlinks = blinks.BySource.ToDictionary(g => g.Key, g => g.Value.Last());
            log.LogInformation($"LastBlinks @ {w.Elapsed.TotalMilliseconds}");

            var currentPower = blinks.GetCurrentPowerConsumption();
            AveragePeriod = currentPower.AveragingPeriod;
            CurrentPowerConsumption = currentPower.Power.ToString("0.0");
            log.LogInformation($"power @ {w.Elapsed.TotalMilliseconds}");

            PeriodConsumption = blinks.BySource
                .Select(g =>
                {
                    var @default = g.Value.All.First();
                    var firstOfYear = g.Value.All.LastOrDefault(b => b.Timestamp.Year == now.Year - 1) ?? @default;
                    var firstOfMonth = g.Value.All.LastOrDefault(b => b.Timestamp.Month != now.Month) ?? @default;
                    var firstOfWeek = g.Value.All.LastOrDefault(b => b.Timestamp < now.StartOfWeek()) ?? @default;
                    var lastPoint = g.Value.Last();
                    return (lastPoint.Source, YearToDate: (lastPoint.Value - firstOfYear.Value),
                        MonthToDate: (lastPoint.Value - firstOfMonth.Value),
                        WeekToDate: (lastPoint.Value - firstOfWeek.Value));
                })
                .ToDictionary(i => i.Source);
            log.LogInformation($"period @ {w.Elapsed.TotalMilliseconds}");

            DailyAverage = blinks.Averages[Source.Power];

        }

        public string BlinkCount { get; }
        public IDictionary<Source, Blink> LastBlinks { get; }
        public string CurrentPowerConsumption { get; }
        public Dictionary<Source, (Source Source, int YearToDate, int MonthToDate, int WeekToDate)> PeriodConsumption { get; }
        public double DailyAverage { get; }
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
