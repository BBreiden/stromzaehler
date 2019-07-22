using System;
using System.Collections.Generic;
using System.Linq;
using Stromzaehler.Models;

namespace Stromzaehler.Analysis 
{
    public class BlinkAnalysis
    {
        public BlinkAnalysis(IBlinkData blinkData)
        {
            BlinkData = blinkData;
        }

        public IBlinkData BlinkData { get; }

        public IEnumerable<Blink> FillGaps() {
            var blinks = BlinkData.Blinks.OrderBy(b => b.Timestamp).ToList();

            if (!blinks.Any())
            {
                yield break;
            }

            // return first
            var prev = blinks.First();
            yield return prev;

            foreach (var blink in BlinkData.Blinks.Skip(1)) 
            {
                var gap = blink.Value - prev.Value;
                if (gap <= 0)
                    throw new InvalidOperationException("Gap must be positive.");

                if (gap == 1) {
                    prev = blink;
                    yield return blink;
                }

                if (gap != 1) {
                    var dt = (blink.Timestamp - prev.Timestamp) / gap;

                    if (dt.Ticks <= 0)
                        throw new InvalidOperationException("Time gap must be positive.");
                    
                    var time = prev.Timestamp;
                    var count = prev.Value;
                    while (count < blink.Value) {
                        time = time + dt;
                        count++;
                        yield return new Blink {Timestamp = time, Value = count};
                    }
                    prev = blink;
                }
            }
        }
    }
}