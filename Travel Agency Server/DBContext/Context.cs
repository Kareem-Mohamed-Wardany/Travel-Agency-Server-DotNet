using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Travel_Agency_Server.Model;

namespace Travel_Agency_Server.DBContext
{
    public class Context: IdentityDbContext<IdentityUser>
    {
        public Context() { }

        public Context(DbContextOptions options):base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripsReverved> TripsReverved { get; set; }
    }
}
