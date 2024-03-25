using BookObserver.ViewModels.Registrator_Locator;
using Microsoft.Extensions.Hosting;
using System.Windows;

namespace BookObserver
{
    public partial class App : Application
    {
        public static Window ActiveWindow => Current.Windows.Cast<Window>().First(p => p.IsActive);

        private static IHost? __host;

        public static IHost Host => __host ??= Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder(Environment.GetCommandLineArgs())
            .ConfigureServices((host, services) => services.AddViewModels()).Build();

        public static IServiceProvider Services => Host.Services;

        protected override void OnStartup(StartupEventArgs e)
        {
            var host = Host;
            base.OnStartup(e);
            host.StartAsync();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            using var host = Host;
            base.OnExit(e);
            host.StopAsync();
        }
    }
}
