using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Stromzaehler.Models;
using Stromzaehler.Tools;

namespace Stromzaehler.Pages
{
    public class DailyConsumptionModel : PageModel
    {
        private readonly IBlinkData bd;
        private readonly ILogger<DailyConsumptionModel> log;

        private List<Blink> Blinks { get; }

        public DailyConsumptionModel(IBlinkData bd, ILogger<DailyConsumptionModel> log)
        {
            this.bd = bd;
            this.log = log;
            Blinks = new List<Blink>();
        }

        public void LoadBlinks(int lastDays)
        {
            var blinks = bd.BySource[Source.Power].All
                .GroupBy(d => d.Timestamp.Date)
                .OrderByDescending(g => g.Key)
                .Take(lastDays)
                .Select(g => g.OrderBy(b => b.Timestamp).Last())
                .OrderBy(b => b.Timestamp);
            Blinks.AddRange(blinks);
            log.LogInformation($"Blinks read: {Blinks.Count}");
        }

        public string GetLabels(int lastDays)
        {
            var labels = Blinks
                .Select(b => b.Timestamp.ToString("yyyy-MM-dd"))
                .ToArray();

            return $"['{string.Join("','", labels)}']";
        }

        /// <summary>
        /// Returns the number of blinks per day of the lastDays.
        /// </summary>
        public string GetEnergyData(int lastDays)
        {
            var values = Blinks
                .Diff((a, b) => b.Value - a.Value);
            return $"[{string.Join(',', values)}]";
        }
    }
}