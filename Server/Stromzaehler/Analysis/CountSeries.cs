using System;
using System.Collections.Generic;
using System.Linq;
using Stromzaehler.Models;

namespace Stromzaehler.Analysis
{
    public class CountSeries
    {
        private List<Blink> blinks;
        private int outOfOrder;
        private int lastValue;  // contains the Value of the last added blink 
        private int currentCount; // contains the count of the last blink in blinks

        public CountSeries()
        {
            lastValue = 0;
            blinks = new List<Blink>();
            outOfOrder = 0;
            currentCount = 0;
        }

        public void Add(Blink blink)
        {
            var delta = blink.Value - lastValue;

            if (delta < 0)
            {
                // it seems to be an out of order blink
                outOfOrder++;

                // if the sequence of out of order blinks is yet to short, ignore it
                if (outOfOrder < 3) return;
                delta = blink.Value;
            }

            currentCount += delta;
            blinks.Add(new Blink
            {
                BlinkId = blink.BlinkId,
                Timestamp = blink.Timestamp,
                Source = blink.Source,
                Value = currentCount
            });

            outOfOrder = 0;
            lastValue = blink.Value;
        }

        public (double average, TimeSpan period) CurrentBlinksPerHour()
        {
            var b1 = blinks[blinks.Count - 1];
            var b0 = blinks[Math.Max(blinks.Count - 10, 0)];
            var period = b1.Timestamp - b0.Timestamp;
            var average = (b1.Value - b0.Value) / period.TotalHours;
            return (average, period);
        }

        public double AverageDailyBlinkCount()
        {
            return blinks.GroupBy(b => b.Timestamp.Date)
                .Select(g => g.Count())
                .Average();
        }

        public Blink Last() => blinks.Last();

        public IEnumerable<Blink> All => blinks;
    }
}