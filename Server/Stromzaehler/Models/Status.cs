using Microsoft.Extensions.Configuration;
using System;
using System.Text.Json;

namespace Stromzaehler.Models
{
    public class Status
    {
        public Status(DateTime bootTime, IConfiguration config)
        {
            BootTime = bootTime;
            Config = JsonSerializer.Serialize(config);
        }

        public DateTime BootTime { get; }

        public string Config { get; }
    }
}
