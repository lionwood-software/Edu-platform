using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace SchoolApi
{
    public class Program
    {
        public static string AppName;
        public static string AppSchoolId;

        public static void Main(string[] args)
        {
            var logger = Log.Logger;

            try
            {
                AppName = "School-" + Environment.GetEnvironmentVariable("SCHOOL_UNIQUE_ID");
                AppSchoolId = Environment.GetEnvironmentVariable("SCHOOL_UNIQUE_ID");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, ex.Message);
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var isPortValid = int.TryParse(Environment.GetEnvironmentVariable("EXPOSED_PORT"), out int port);

                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, isPortValid ? port : 5000);
                    });
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog((context, config) =>
                    {
                        config
                        .Enrich.FromLogContext()
                        .WriteTo.File(Path.Combine(Environment.CurrentDirectory, "logs", "log.txt"), rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose);
                    });
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                });
    }
}
