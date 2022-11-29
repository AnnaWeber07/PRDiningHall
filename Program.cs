using AnnaWebDiningFin.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AnnaWebDiningFin
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<DinningHallStartup, DinningHallStartup>();
                    services.AddHostedService<Dinning>();
                }).Build().Run();
        }
    }
}