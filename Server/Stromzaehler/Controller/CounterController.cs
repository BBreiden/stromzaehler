using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stromzaehler.Models;

namespace Stromzaehler.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CounterController : ControllerBase
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
        public async Task PostAsync([FromBody] PostData data)
        {
            if (!ModelState.IsValid)
            {
                throw new ArgumentException("Invalid model.");
            }

            blinkData.Blinks.Add(new Blink() { Value = data.Count, Timestamp = DateTimeOffset.Now });
            await blinkData.SaveChangesAsync();
        }

        public class PostData
        {
            public int Count { get; set; }
        }
    }
}
