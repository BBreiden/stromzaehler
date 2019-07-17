using System;
using System.Collections.Generic;
using System.Linq;

namespace Stromzaehler.Models
{
    public class Blink
    {
        public long BlinkId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int Value { get; set; }
    }
}
