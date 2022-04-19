using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;
using System.Net;

namespace RouterApi
{
    public class Program
    {
        public static readonly string AppName = "Router";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, 5000);
                    });
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog((context, config) =>
                    {
                        config
                        .Enrich.FromLogContext()
                        .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "logs", "log.txt"), rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose);
                    });
                });
    }
}
