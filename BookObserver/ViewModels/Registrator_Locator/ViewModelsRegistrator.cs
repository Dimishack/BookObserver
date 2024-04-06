using BookObserver.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace BookObserver.ViewModels.Registrator_Locator
{
    public static class ViewModelsRegistrator
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services) => services
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<BooksViewModel>()
            .AddSingleton<ReadersViewModel>()
            .AddTransient<CreatorBookViewModel>()
            .AddTransient(
            s =>
            {
                var viewModel = s.GetRequiredService<CreatorBookViewModel>();
                var window = new CreatorBookWindow { DataContext = viewModel };
                window.Closed += (_, _) => viewModel = null;
                window.Owner = App.ActiveWindow;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                return window;
            })
            .AddTransient<CreatorReaderViewModel>()
            .AddTransient(
            s =>
            {
                var viewModel = s.GetRequiredService<CreatorReaderViewModel>();
                var window = new CreatorReaderWindow { DataContext = viewModel };
                window.Closed += (_, _) => viewModel = null;
                window.Owner = App.ActiveWindow;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                return window;
            })
            .AddTransient<EditorBookViewModel>()
            .AddTransient(
            s =>
            {
                var viewModel = s.GetRequiredService<EditorBookViewModel>();
                var window = new EditorBookWindow { DataContext = viewModel };
                window.Closed += (_, _) => viewModel = null;
                window.Owner = App.ActiveWindow;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                return window;
            })
            .AddTransient<EditorReaderViewModel>()
            .AddTransient(
            s =>
            {
                var viewModel = s.GetRequiredService<EditorReaderViewModel>();
                var window = new EditorReaderWindow { DataContext = viewModel };
                window.Closed += (_, _) => viewModel = null;
                window.Owner = App.ActiveWindow;
                window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                return window;
            })
            ;
    }
}
