using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Demo.RentalRepairs.WebMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    // Add user secrets in development
                    if (context.HostingEnvironment.IsDevelopment())
                    {
                        config.AddUserSecrets(typeof(Program).Assembly);
                    }
                })
                .UseStartup<Startup>();
    }
}
