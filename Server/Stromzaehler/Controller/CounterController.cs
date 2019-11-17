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
        private readonly BlinkDataContext blinkDb;
        private readonly IBlinkData blinkData;

        public CounterController(ILogger<CounterController> log, BlinkDataContext db, IBlinkData blinkData)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            blinkDb = db;
            this.blinkData = blinkData;
        }

        [HttpGet()]
        public IEnumerable<Blink> Get()
        {
            return blinkDb.Blinks.ToArray();
        }

        [HttpPost()]
        public async Task<IActionResult> PostAsync([FromBody] PostData data)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException("Invalid model.");
            }

            if (data.Source == Source.None)
            {
                return BadRequest("Invalid source: " + data.SourceString ?? "<null>");
            }

            var blink = new Blink()
            {
                Source = data.Source,
                Timestamp = DateTimeOffset.Now,
                Value = data.Count
            };

            // save to db
            blinkDb.Blinks.Add(blink);
            await blinkDb.SaveChangesAsync();

            // update analysis
            blinkData.Update(blink);
            return Ok();
        }
    }
}
