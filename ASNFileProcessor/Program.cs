using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ASNFileProcessor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args).ConfigureServices((hostContext, services) =>
            {
                services.AddDbContext<ShippingDbContext>(options =>
                {
                    options.UseSqlServer("Server=.\\SQLExpress;Database=ShippingDB;Integrated Security=True;TrustServerCertificate=True;");
                });
                services.AddScoped<ASNFileProcessor>();

                services.AddScoped<FileWatcher>();
            }).Build();

            var fileWatcher = host.Services.GetRequiredService<FileWatcher>();
            fileWatcher.StartWatching();

            await Task.Delay(-1);
        }
    }
}