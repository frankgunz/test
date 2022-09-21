using Hubbell.EHubb.Common.Logger;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace Hubbell.EHubb.BACnetAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.CaptureStartupErrors(true);
                    webBuilder.UseContentRoot(AppContext.BaseDirectory);
                    webBuilder.UseStartup<Startup>()
                        .ConfigureAppConfiguration((hostingContext, config) =>
                        {
                            config.AddJsonFile(
                                $"bacnetapi.appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.log.json",
                                true, true);
                        });
                    webBuilder.UseUrls("http://*:5007");
                }).UseSerilog(SerilogConfiguration.Configure);
    }
}
