using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stromzaehler.Models;
using Stromzaehler.Tools;

namespace Stromzaehler.Pages
{
    public class PowerChartModel : PageModel
    {
        private IQueryable<Blink> blinks;
        private IReadOnlyCollection<Blink> blinksLoaded;
        private List<Blink> blinksLoadedAll;
        private readonly BlinkDataContext db;
        private readonly ILogger<PowerChartModel> log;

        public PowerChartModel(BlinkDataContext db, ILogger<PowerChartModel> log)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.log = log;
            log.LogInformation("PowerChartModel initialized.");
        }

        public async Task OnGetAsync()
        {
            blinks = db.Blinks.AsNoTracking();
        }

        public void LoadBlinks(int lastHours)
        {
            blinksLoaded = GetBlinksSkipped(lastHours);
        }

        public string GetLabels()
        {
            log.LogTrace("Getting labels");
            var labels = blinksLoaded
                .Select(b => b.Timestamp.ToString("yyyy-MM-ddTHH:mm"))
                .ToArray();
            var result = $"['{string.Join("','", labels)}']";
            log.LogTrace("Got labels:" + result.Length);
            return result;
        }

        public string GetPowerDataRough(int lastHours)
        {
            var values = GetBlinksSkipped(lastHours)
                // .TakeLast(count)
                .Diff((next, prev) =>
                3600 * (next.Value - prev.Value) / (next.Timestamp - prev.Timestamp).TotalSeconds);
            return $"[{string.Join(',', values)}]";
        }

        public string GetPowerData()
        {
            log.LogTrace("Getting power data.");
            var values = blinksLoaded
                .Diff((next, prev) =>
                3600 * (next.Value - prev.Value) / (next.Timestamp - prev.Timestamp).TotalSeconds);
            var result = $"[{string.Join(',', values)}]";
            log.LogTrace("Got power data: " + result.Length);
            return result;
        }

        public string GetEnergyData()
        {
            var values = blinksLoaded
                .Select(e => e.Value / 1000.0);
            return $"[{string.Join(',', values)}]";
        }

        public string GetEnergyDataAsHistogram() {
            var result = GetHistogram();
            return $"[{string.Join(',', result.Select(d => d.Item2))}]";
        }

        public IEnumerable<(DateTimeOffset, double)> GetHistogram() {
            var allBlinks = blinksLoadedAll;
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
            blinksLoadedAll = GetBlinks(lastHours).ToList();
            var skip = blinksLoadedAll.Count / 100;
            return blinksLoadedAll.Where((e, i) => i % skip == 0).ToList();
        }

        /// <summary>
        /// Returns the blinks of the lastHours ordered by time stamp
        /// </summary>
        /// <param name="lastHours"></param>
        /// <returns></returns>
        public IQueryable<Blink> GetBlinks(int lastHours) {
            var from = DateTime.Now.AddHours(-lastHours);
            //var result = blinks
            //    .Where(b => b.Timestamp > DateTimeOffset.Now.AddHours(-lastHours))
            //    .OrderBy(b => b.Timestamp);
            var result = db.Blinks
                .FromSql($"Select * from blinks where datetime(timestamp) > datetime({from})");
            return result;
        }
    }
}