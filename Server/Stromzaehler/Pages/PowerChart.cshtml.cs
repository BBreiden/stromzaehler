using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Stromzaehler.Models;
using Stromzaehler.Tools;

namespace Stromzaehler.Pages
{
    public class PowerChartModel : PageModel
    {
        private IQueryable<Blink> blinks;
        private readonly BlinkDataContext db;

        public PowerChartModel(BlinkDataContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task OnGetAsync()
        {
            blinks = db.Blinks.AsNoTracking();
        }

        public string GetLabels(int lastHours)
        {
            var labels = GetBlinksSkipped(lastHours)
                .Select(b => b.Timestamp.ToString("yyyy-MM-ddTHH:mm"))
                .ToArray();
            return $"['{string.Join("','", labels)}']";
        }

        public string GetPowerDataRough(int lastHours)
        {
            var values = GetBlinksSkipped(lastHours)
                // .TakeLast(count)
                .Diff((next, prev) =>
                3600 * (next.Value - prev.Value) / (next.Timestamp - prev.Timestamp).TotalSeconds);
            return $"[{string.Join(',', values)}]";
        }

        public string GetPowerData(int lastHours)
        {
            var values = GetBlinksSkipped(lastHours)
                .Diff((next, prev) =>
                3600 * (next.Value - prev.Value) / (next.Timestamp - prev.Timestamp).TotalSeconds);
            return $"[{string.Join(',', values)}]";
        }

        public string GetEnergyData(int lastHours)
        {
            var values = GetBlinksSkipped(lastHours)
                .Select(e => e.Value / 1000.0);
            return $"[{string.Join(',', values)}]";
        }

        public string GetEnergyDataAsHistogram(int lastHours) {
            var result = GetHistogram(lastHours);
            return $"[{string.Join(',', result.Select(d => d.Item2))}]";
        }

        public IEnumerable<(DateTimeOffset, double)> GetHistogram(int lastHours) {
            var allBlinks = GetBlinks(lastHours);
            var hist = new Histogram(allBlinks
                .Select(b => (double)b.Timestamp.Ticks), 100);
            var offset = allBlinks.First().Timestamp.Offset;
            for (var i = 0; i<hist.BucketCount; i++) {
                long x = (long)(hist[i].LowerBound + 0.5 * hist[i].Width);
                yield return (new DateTimeOffset(x, offset), hist[i].Count); 
            }
        }

        /// <summary>
        /// Returns 100 blinks taken from the blinks of the lastHours
        /// </summary>
        public IReadOnlyCollection<Blink> GetBlinksSkipped(int lastHours) {
            var result = GetBlinks(lastHours).ToList();
            var skip = result.Count / 100;
            return result.Where((e, i) => i % skip == 0).ToList();
        }

        /// <summary>
        /// Returns the blinks of the lastHours ordered by time stamp
        /// </summary>
        /// <param name="lastHours"></param>
        /// <returns></returns>
        public IQueryable<Blink> GetBlinks(int lastHours) {
            var result = blinks
                .Where(b => b.Timestamp > DateTimeOffset.Now.AddHours(-lastHours))
                .OrderBy(b => b.Timestamp);
            return result;
        }
    }
}