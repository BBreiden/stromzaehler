using System;
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
        private readonly CounterModel counter = new CounterModel();

        public CounterController(ILogger<CounterController> log, CounterModel counter)
        {
            this.log = log ?? throw new ArgumentNullException(nameof(log));
            this.counter = counter;
        }

        [HttpGet("get")]
        public CounterModel Get()
        {
            return counter;
        }

        [HttpGet("inc")]
        public CounterModel Increase() 
        {
            counter.Count++;
            counter.Blinks.Add(DateTimeOffset.Now);
            return counter;
        }
    }
}
