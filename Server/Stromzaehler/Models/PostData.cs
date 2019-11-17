using System;
using Stromzaehler.Models;

namespace Stromzaehler.Models
{
    public class PostData
    {
        // name of the sensor e.g. 'power', 'water'
        public string? SourceString { get; set; }
        public int Count { get; set; }

        public Source Source
        {
            get
            {
                if (Enum.TryParse<Source>(SourceString, out var src))
                {
                    return src;
                }
                return Source.None;
            }
        }
    }
}
