using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stromzaehler.Models;
using Stromzaehler.Tools;

namespace Stromzaehler.Pages
{
    public class PowerChartModel : PageModel
    {
        private List<Blink> blinks;
        private readonly BlinkDataContext db;

        public PowerChartModel(BlinkDataContext db)
        {
            this.db = db ?? throw new System.ArgumentNullException(nameof(db));
        }

        public async Task OnGetAsync()
        {
            blinks = await db.Blinks.AsNoTracking().ToListAsync();
        }

        public string GetLabels(int lastHours)
        {
            var labels = GetBlinks(lastHours)
                .Select(b => b.Timestamp.ToString("yyyy-MM-ddTHH:mm"))
                .ToArray();
            return $"['{string.Join("','", labels)}']";
        }

        public string GetPowerDataRough(int lastHours)
        {
            var values = GetBlinks(lastHours)
                // .TakeLast(count)
                .Diff((next, prev) =>
                3600 * (next.Value - prev.Value) / (next.Timestamp - prev.Timestamp).TotalSeconds);
            return $"[{string.Join(',', values)}]";
        }

        public string GetPowerData(int lastHours)
        {
            var values = GetBlinks(lastHours)
                .Diff((next, prev) =>
                3600 * (next.Value - prev.Value) / (next.Timestamp - prev.Timestamp).TotalSeconds);
            return $"[{string.Join(',', values)}]";
        }

        public string GetEnergyData(int lastHours)
        {
            var values = GetBlinks(lastHours)
                .Select(e => e.Value / 1000.0);
            return $"[{string.Join(',', values)}]";
        }

        public IEnumerable<Blink> GetBlinks(int lastHours) {
            var result = blinks.Where(b => b.Timestamp > DateTimeOffset.Now.AddHours(-lastHours))
                .OrderBy(b => b.Timestamp)
                .ToArray();
            var skip = result.Length / 100;
            return result.Where((e, i) => i % skip == 0);
        }
    }
}