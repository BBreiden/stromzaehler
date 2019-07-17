using Microsoft.EntityFrameworkCore;

namespace Stromzaehler.Models
{
    public class BlinkDataContext : DbContext
    {
        public BlinkDataContext(DbContextOptions<BlinkDataContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Blink> Blinks { get; set; }
    }
}
