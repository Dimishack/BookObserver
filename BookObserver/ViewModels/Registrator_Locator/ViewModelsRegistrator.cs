using Microsoft.Extensions.DependencyInjection;

namespace BookObserver.ViewModels.Registrator_Locator
{
    public static class ViewModelsRegistrator
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services) => services
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<BooksViewModel>()
            .AddSingleton<ReadersViewModel>()
            ;
    }
}
