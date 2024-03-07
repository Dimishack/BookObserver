﻿using Microsoft.Extensions.Hosting;
using System;
using System.Windows;
using BookObserver.ViewModels.Registrator;

namespace BookObserver
{
    public partial class App : Application
    {
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
