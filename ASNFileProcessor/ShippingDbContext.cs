using Microsoft.EntityFrameworkCore;

namespace ASNFileProcessor
{
    // Class which allows to work with the database
    public class ShippingDbContext : DbContext
    {
        // Allows to access two tables in the database
        public DbSet<ASNHeader> ASNHeaders { get; set; }
        public DbSet<ASNLine> ASNLines { get; set; }

        public ShippingDbContext(DbContextOptions<ShippingDbContext> options) : base(options)
        {
        }
    }

}