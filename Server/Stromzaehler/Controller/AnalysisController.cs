
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stromzaehler.Models;

namespace Stromzaehler.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly IBlinkData blinkData;

        public AnalysisController(IBlinkData bd) 
        {
            this.blinkData = bd ?? throw new ArgumentNullException(nameof(bd));
        }

        // [HttpGet]
        // public async Task<IEnumerable<(int Index, long Count)>> GetBinned() 
        // {
        //     blinkData.Blinks.AsNoTracking()
                
        // }

        public IEnumerable<Blink> FillGaps() {
            var blinks = blinkData.Blinks.OrderBy(b => b.Timestamp).ToList();

            // return first
            var prev = blinks.First();
            yield return prev;

            foreach (var blink in blinkData.Blinks.Skip(1)) 
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