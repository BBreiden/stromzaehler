using System;
using System.Collections.Generic;
using System.Linq;
using Stromzaehler.Analysis;
using Stromzaehler.Controller;
using Stromzaehler.Models;
using Xunit;
using Xunit.Abstractions;
using static Xunit.Assert;

namespace Stromzaehler.Tests
{
    public class FillGapTests
    {
        private readonly ITestOutputHelper output;

        public FillGapTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Empty_list_works()
        {
            var data = new Blink[0];
            var c = new BlinkAnalysis(new TestData(data));
            Equal(0, c.FillGaps().Count());
        }

        [Fact]
        public void One_blink_is_not_changed()
        {
            var data = new[] 
            {
                new Blink { Timestamp = DateTimeOffset.Now, Value = 1}
            };
            var c = new BlinkAnalysis(new TestData(data));
            Equal(1, c.FillGaps().Count());
        }

        [Fact]
        public void Gap_of_1_is_not_changed()
        {
            var time = DateTimeOffset.Now;
            var data = new[] 
            {
                new Blink { Timestamp = time, Value = 1},
                new Blink { Timestamp = time, Value = 2}
            };

            output.WriteLine("Setup");
            var filled = new BlinkAnalysis(new TestData(data))
                .FillGaps()
                .ToArray();
            output.WriteLine("Check");
            Equal(2, filled.Length);
            Equal(data[0], filled[0]);
            Equal(data[1], filled[1]);
        }

        [Theory]
        [InlineData(2, 10)]
        [InlineData(2, 75)]
        [InlineData(17, 111)]
        public void Gap_of_2_is_handled_correctly(int countGap, int ticks)
        {
            output.WriteLine("Setup");
            var time = DateTimeOffset.Now;
            var data = new[] 
            {
                new Blink { Timestamp = time, Value = 1},
                new Blink { Timestamp = time.AddTicks(ticks), Value = 1 + countGap}
            };
            foreach (var b in data) 
            {
                output.WriteLine(b.ToString());
            }

            output.WriteLine("Run");
            var filled = new BlinkAnalysis(new TestData(data))
                .FillGaps()
                .ToArray();
            foreach (var b in filled) 
            {
                output.WriteLine(b.ToString());
            }

            output.WriteLine("Check");
            Equal(countGap + 1, filled.Length);
            Equal(data[0], filled[0]);
            Equal(data[1].Value, filled[countGap].Value);
        }

        private class TestData : IBlinkData
        {
            private readonly IEnumerable<Blink> data;

            public TestData(IEnumerable<Blink> data) 
            {
                this.data = data;
            }
            
            public IQueryable<Blink> Blinks
            {
                get
                {
                    return data.AsQueryable();
                }
            }
        }
    }
}