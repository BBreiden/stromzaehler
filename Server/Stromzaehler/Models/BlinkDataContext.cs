using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Stromzaehler.Models
{
    public class BlinkDataContext : DbContext, IBlinkData 
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

        IQueryable<Blink> IBlinkData.Blinks => Blinks;
    }

    public interface IBlinkData
    {
        IQueryable<Blink> Blinks { get; }
    }
}
