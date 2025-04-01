using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace ASNFileProcessor
{
    public class DbContextFactory : IDesignTimeDbContextFactory<ShippingDbContext>
    {
        public ShippingDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShippingDbContext>();
            optionsBuilder.UseSqlServer("Server=.\\SQLExpress;Database=ShippingDB;Integrated Security=True;TrustServerCertificate=True;");

            return new ShippingDbContext(optionsBuilder.Options);
        }
    }
}
