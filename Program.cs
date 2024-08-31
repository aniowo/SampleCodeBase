using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdditionalAccountCreation.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
                  Host.CreateDefaultBuilder(args).UseWindowsService().ConfigureServices((hostContext, services) =>
                      {

                          IConfiguration configuration = hostContext.Configuration;
                          ConnectionManager connection = configuration.GetSection("ConnectionStrings").Get<ConnectionManager>();
                          services.AddSingleton(connection);
                          services.AddHostedService<Worker>();
                          services.AddSingleton<IBasisRepo, BasisRepo>();                   
                          services.AddTransient<SelectRecords>();
                          services.AddTransient<BasisCheck>();
                          services.AddTransient<ProcessProfiling>();
                          services.AddTransient<InsertRecords>();



                      }).UseSerilog((hostingContext, LoggerConfiguration) => {
                          LoggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
                      });
    }
}
