using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Stromzaehler.Models
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class CounterController : ControllerBase
    {
        private readonly ILogger<CounterController> log;
        private readonly BlinkDataContext blinkData;

        public CounterController(ILogger<CounterController> log, BlinkDataContext db)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            blinkData = db;
        }

        [HttpGet()]
        public IEnumerable<Blink> Get()
        {
            return blinkData.Blinks.ToArray();
        }

        [HttpPost()]
        public async Task<IActionResult> PostAsync([FromBody] PostData data)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException("Invalid model.");
            }

            var blink = new Blink()
            {
                Source = data.Source,
                Timestamp = DateTimeOffset.Now,
                Value = data.Count
            };

            switch (blink.Source)
            {
                case Source.Power:
                case Source.Water:
                    blinkData.Blinks.Add(blink);
                    break;
                default:
                    return BadRequest($"Invalid data source type: {data.SourceString ?? "<null>"}");
            }
            
            await blinkData.SaveChangesAsync();
            return Ok();
        }
    }
}
