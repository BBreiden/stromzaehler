using System;
using System.Collections.Generic;
using System.Linq;

namespace Stromzaehler.Models
{
    public class CounterModel
    {
        public int Count { get; set; }
        public List<DateTimeOffset> Blinks { get; set; } = new List<DateTimeOffset>();
        public IEnumerable<DateTimeOffset> GetReversedBlinks()
        {
            var result = Blinks.ToList();
            result.Reverse();
            return result;
        }
    }
}
