using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stromzaehler.Models;

namespace Stromzaehler.Analysis
{
    public class BlinkAnalysis : IBlinkData
    {
        public BlinkAnalysis(BlinkDataContext blinkData, ILogger<BlinkAnalysis> log)
        {
            log.LogInformation("Loading data.");
            // get all blinks from database
            var blinks = blinkData.Blinks
                .AsNoTracking()
                .ToList()
                .OrderBy(b => b.Timestamp)
                .ToList();
            Count = blinks.Count;
            log.LogInformation("Done loading data.");

            var grouped = blinks.GroupBy(b => b.Source)
                .ToDictionary(b => b.Key, b => b.ToList());
            BySource = CleanupHistory(grouped);

            Averages = BySource.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.AverageDailyBlinkCount());
        }

        // Group by source and make history monotonous growing.
        // The sequence can be slightly unordered, e.g. 100, 101, 102, _90_, 103.
        // The value 90 is probably just out of order and does not signal a reset
        // of the counter. Therefore it can be ignored.
        //
        // Assumes the Blink list are sorted chronologically.
        internal Dictionary<Source, CountSeries> CleanupHistory(Dictionary<Source, List<Blink>> grouped)
        {
            var result = new Dictionary<Source, CountSeries>();

            foreach (var src in grouped.Keys)
            {
                var series = new CountSeries();
                foreach (var blink in grouped[src])
                {
                    series.Add(blink);
                }
                result[src] = series;
            }

            return result;
        }

        public int Count { get; private set; }
        public IReadOnlyDictionary<Source, CountSeries> BySource { get; }
        public IReadOnlyDictionary<Source, double> Averages { get; }

        public (double Power, TimeSpan AveragingPeriod) GetCurrentPowerConsumption() 
            => BySource[Source.Power].CurrentBlinksPerHour();

        public void Update(Blink blink)
        {
            // increase counter
            Count++;

            BySource[blink.Source].Add(blink);

        }
    }
}