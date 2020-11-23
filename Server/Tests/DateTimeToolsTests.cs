using Stromzaehler.Tools;
using System;
using Xunit;

namespace Tests
{
    public class DateTimeToolsTests
    {
        [Fact]
        public void Sunday_is_day_0()
        {
            var now = DateTimeOffset.Parse("10/11/2019 16:21:07 +01:00");
            Assert.Equal(0, (int)now.DayOfWeek);
        }

        [Fact]
        public void Gets_correct_start_of_week()
        {
            var now = DateTimeOffset.Parse("10/11/2019 16:21:07 +01:00");

            var expectation = DateTimeOffset.Parse("04/11/2019 00:00:00 +01:00");
            var start = now.StartOfWeek();
            Assert.Equal(expectation, start);
        }
    }
}
