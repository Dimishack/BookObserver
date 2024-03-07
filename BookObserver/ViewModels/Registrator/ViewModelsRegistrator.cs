using Microsoft.Extensions.DependencyInjection;

namespace BookObserver.ViewModels.Registrator
{
    public static class ViewModelsRegistrator
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services) => services
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<BooksUserControlViewModel>()
            ;
    }
}
