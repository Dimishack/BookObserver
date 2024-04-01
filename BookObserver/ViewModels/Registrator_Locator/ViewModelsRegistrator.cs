using BookObserver.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

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
                return window;
            })
            ;
    }
}
