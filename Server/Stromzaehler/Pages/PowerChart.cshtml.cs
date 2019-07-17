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

        public string GetLabels()
        {
            return $"['{string.Join("','", blinks.Select(b => b.Timestamp.ToString()).ToArray())}']";
        }

        public string GetData()
        {
            var values = blinks
                .Diff((next, prev) => 
                3600 * (next.Value - prev.Value) / (prev.Timestamp - next.Timestamp).TotalSeconds);
            return $"[{string.Join(',', values)}]";
        }
    }
}