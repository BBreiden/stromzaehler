using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Stromzaehler.Analysis;

namespace Stromzaehler.Models
{
    public class BlinkDataContext : DbContext 
    {
        public BlinkDataContext(DbContextOptions<BlinkDataContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Blink>()
                .HasIndex(b => new { b.Timestamp, b.Source });
        }

        public DbSet<Blink> Blinks { get; set; }

    }

    public interface IBlinkData
    {
        int Count { get; }
        IReadOnlyDictionary<Source, CountSeries> BySource { get; }
        IReadOnlyDictionary<Source, double> Averages { get; }
        (double Power, TimeSpan AveragingPeriod) GetCurrentPowerConsumption();
        public void Update(Blink blink);
    }
}
