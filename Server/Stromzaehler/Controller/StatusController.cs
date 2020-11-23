using Microsoft.AspNetCore.Mvc;
using Stromzaehler.Models;
using System;

namespace Stromzaehler.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly Status status;

        public StatusController(Status status)
        {
            this.status = status;
        }

        [HttpGet]
        public ActionResult<string> GetStatus()
        {
            var uptime = (DateTime.Now - status.BootTime);
            return $"Healthy\n uptime {uptime.Days} days {uptime.Hours} hours and {uptime.Minutes} minutes";
        }
    }
}
