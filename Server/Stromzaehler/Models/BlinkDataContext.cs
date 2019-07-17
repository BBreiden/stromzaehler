using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
