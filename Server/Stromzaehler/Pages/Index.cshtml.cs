using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stromzaehler.Models;

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

            var n = 10;
            var lastBlinks = database.Blinks.AsNoTracking()
                .OrderByDescending(b => b.Timestamp)
                .Take(n)
                .ToArray();

            LastBlinks = lastBlinks[0];
            LastBlinkCount = LastBlinks.Value.ToString("##,#");
            AveragePeriod = (lastBlinks[0].Timestamp - lastBlinks[n-1].Timestamp);
            var power = (lastBlinks[0].Value - lastBlinks[n-1].Value) / AveragePeriod.TotalHours;
            CurrentConsumption = power.ToString("0.0");

        }

        public string BlinkCount { get; }
        public string LastBlinkCount { get; }
        public Blink LastBlinks { get; }
        public string CurrentConsumption { get; }
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
