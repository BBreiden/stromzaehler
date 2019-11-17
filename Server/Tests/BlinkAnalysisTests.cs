using Stromzaehler.Analysis;
using Stromzaehler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class BlinkAnalysisTests
    {
        [Fact]
        public void Regular_series_is_unchanged()
        {
            var series = new CountSeries();
            series.Add(new Blink { Source = Source.Power, BlinkId = 1, Value = 10 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 2, Value = 11 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 3, Value = 12 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 4, Value = 13 });

            var res = series.All.Select(b => b.Value).ToList();
            Assert.Equal(10, res[0]);
            Assert.Equal(11, res[1]);
            Assert.Equal(12, res[2]);
            Assert.Equal(13, res[3]);
        }

        [Fact]
        public void Reset_recognized()
        {
            var series = new CountSeries();
            series.Add(new Blink { Source = Source.Power, BlinkId = 1, Value = 10 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 2, Value = 11 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 3, Value = 12 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 4, Value = 1 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 5, Value = 2 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 6, Value = 3 });

            var res = series.All.Select(b => b.Value).ToArray();
            Assert.Equal(4, res.Length);
            Assert.Equal(10, res[0]);
            Assert.Equal(11, res[1]);
            Assert.Equal(12, res[2]);
            Assert.Equal(15, res[3]);
        }

        [Fact]
        public void Out_of_order_value_is_ignored()
        {
            var series = new CountSeries();
            series.Add(new Blink { Source = Source.Power, BlinkId = 1, Value = 10 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 2, Value = 11 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 3, Value = 12 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 4, Value = 1 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 5, Value = 13 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 6, Value = 15 });

            var res = series.All.Select(b => b.Value).ToArray();
            Assert.Equal(5, res.Length);
            Assert.Equal(10, res[0]);
            Assert.Equal(11, res[1]);
            Assert.Equal(12, res[2]);
            Assert.Equal(13, res[3]);
            Assert.Equal(15, res[4]);
        }

        [Fact]
        public void Out_of_order_sequence_is_ignored()
        {
            var series = new CountSeries();
            series.Add(new Blink { Source = Source.Power, BlinkId = 1, Value = 10 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 2, Value = 11 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 3, Value = 12 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 4, Value = 1 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 5, Value = 3 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 6, Value = 99 });

            var res = series.All.Select(b => b.Value).ToArray();
            Assert.Equal(4, res.Length);
            Assert.Equal(10, res[0]);
            Assert.Equal(11, res[1]);
            Assert.Equal(12, res[2]);
            Assert.Equal(99, res[3]);
        }

        [Fact]
        public void Multiple_out_of_order_sequences_are_corrected()
        {
            var series = new CountSeries();
            series.Add(new Blink { Source = Source.Power, BlinkId = 1, Value = 10 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 2, Value = 1 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 3, Value = 11 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 4, Value = 2 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 5, Value = 15 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 6, Value = 3 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 6, Value = 4 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 6, Value = 16 });


            var res = series.All.Select(b => b.Value).ToArray();
            Assert.Equal(4, res.Length);
            Assert.Equal(10, res[0]);
            Assert.Equal(11, res[1]);
            Assert.Equal(15, res[2]);
            Assert.Equal(16, res[3]);
        }

        [Fact]
        public void Multiple_reset_are_corrected()
        {
            var series = new CountSeries();
            series.Add(new Blink { Source = Source.Power, BlinkId = 1, Value = 10 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 2, Value = 1 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 3, Value = 2 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 4, Value = 3 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 5, Value = 10 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 6, Value = 1 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 6, Value = 2 });
            series.Add(new Blink { Source = Source.Power, BlinkId = 6, Value = 3 });

            var res = series.All.Select(b => b.Value).ToArray();
            Assert.Equal(4, res.Length);
            Assert.Equal(10, res[0]);
            Assert.Equal(13, res[1]);
            Assert.Equal(20, res[2]);
            Assert.Equal(23, res[3]);
        }
    }
}
