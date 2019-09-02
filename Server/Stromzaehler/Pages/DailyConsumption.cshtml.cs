using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stromzaehler.Models;
using Stromzaehler.Tools;

namespace Stromzaehler.Pages
{
    public class DailyConsumptionModel : PageModel
    {
        public BlinkDataContext BlinkData { get; }

        public DailyConsumptionModel(BlinkDataContext blinkData)
        {
            BlinkData = blinkData ?? throw new ArgumentNullException(nameof(blinkData));
        }

        public string GetLabels(int lastDays)
        {
            var labels = GetBlinks(lastDays)
                .Select(b => b.Timestamp.ToString("yyyy-MM-dd"))
                .ToArray();
            return $"['{string.Join("','", labels)}']";
        }

        /// <summary>
        /// Returns the number of blinks per day of the lastDays.
        /// </summary>
        public string GetEnergyData(int lastDays)
        {
            var values = GetBlinks(lastDays)
                .Diff((a, b) => b.Value - a.Value);
            return $"[{string.Join(',', values)}]";
        }

        private IEnumerable<Blink> GetBlinks(int lastDays)
        {
            return BlinkData.Blinks
                .Where(b => b.Timestamp > DateTimeOffset.Now.AddDays(-lastDays - 1))
                .GroupBy(b => b.Timestamp.Date)
                .Select(g => g.OrderBy(b => b.Timestamp).Last());
        }
    }
}