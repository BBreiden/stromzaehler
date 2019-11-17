using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stromzaehler.Models;
using Stromzaehler.Tools;

namespace Stromzaehler.Pages
{
    public class DailyConsumptionModel : PageModel
    {
        public BlinkDataContext BlinkData { get; }
        private List<Blink> Blinks { get; }

        public DailyConsumptionModel(BlinkDataContext blinkData)
        {
            BlinkData = blinkData ?? throw new ArgumentNullException(nameof(blinkData));
            Blinks = new List<Blink>();
        }

        public void LoadBlinks(int lastDays)
        {
            Blinks.AddRange(GetBlinks(lastDays));
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

        private IEnumerable<Blink> GetBlinks(int lastDays)
        {
            return BlinkData.Blinks
                .FromSql($"select * from Blinks where date(timestamp) > date({DateTimeOffset.Now.AddDays(-lastDays)})")
                //.Where(b => b.Timestamp > DateTimeOffset.Now.AddDays(-lastDays))
                .GroupBy(b => b.Timestamp.Date)
                .Select(g => g.OrderBy(b => b.Timestamp).Last());
        }
    }
}