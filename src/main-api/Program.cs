using System;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Serilog;

namespace NitApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = Log.Logger;

            try
            {
                var globalOcelotConfigReplace = Environment.GetEnvironmentVariable("GLOBAL_OCELOT_CONFIG");

                if (string.IsNullOrEmpty(globalOcelotConfigReplace))
                {
                    throw new Exception("GLOBAL_OCELOT_CONFIG not found or is empty!");
                }

                File.WriteAllText("ocelot/main/ocelot.global.json", globalOcelotConfigReplace);
                File.WriteAllText("ocelot/dev/ocelot.global.json", globalOcelotConfigReplace);

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, ex.Message);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
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
                        config.WriteTo.File(Path.Combine(Environment.CurrentDirectory, "logs", "log.txt"), rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose);
                    });

                    webBuilder.ConfigureAppConfiguration((hostingContext, ic) =>
                    {

                        var isValidOcelotDockerUsage = bool.TryParse(Environment.GetEnvironmentVariable("OCELOT_RUN_DOCKER"), out bool ocelotDockerUsage);
                        if (!hostingContext.HostingEnvironment.IsDevelopment() || (isValidOcelotDockerUsage && ocelotDockerUsage))
                        {
                            ic.AddOcelot("ocelot/main", hostingContext.HostingEnvironment);
                        }
                        else
                        {
                            ic.AddOcelot("ocelot/dev", hostingContext.HostingEnvironment);
                        }

                        ic.AddEnvironmentVariables();
                    });
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                });
    }
}
