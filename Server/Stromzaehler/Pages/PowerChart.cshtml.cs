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
            //.TakeLast(count)
            .Select(b => b.Timestamp.ToString("dd/MM HH:mm"))
            .ToArray();
            return $"['{string.Join("','", labels)}']";
        }

        public string GetData(int lastHours)
        {
            var values = GetBlinks(lastHours)
                // .TakeLast(count)
                .Diff((next, prev) =>
                3600 * (next.Value - prev.Value) / (next.Timestamp - prev.Timestamp).TotalSeconds);
            return $"[{string.Join(',', values)}]";
        }

        public IEnumerable<Blink> GetBlinks(int lastHours) {
            return blinks.Where(b => b.Timestamp > DateTimeOffset.Now.AddHours(-lastHours))
            .OrderBy(b => b.Timestamp);
        }
    }
}