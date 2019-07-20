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

        public DbSet<Blink> Blinks { get; set; }

        IQueryable<Blink> IBlinkData.Blinks => Blinks;
    }

    public interface IBlinkData
    {
        IQueryable<Blink> Blinks { get; }
    }
}
