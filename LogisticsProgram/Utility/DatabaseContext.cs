using System.Data.Entity;

namespace LogisticsProgram
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("DefaultConnection")
        {
        }

        public DbSet<Place> Places { get; set; }
        public DbSet<Address> Addresses { get; set; }
    }
}