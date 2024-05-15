using BookObserver.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace BookObserver.Services.Registrator
{
    internal static class ServicesRegistrator
    {
        public static IServiceCollection AddServices(this IServiceCollection services) =>
            services.AddSingleton<IUserDialog, UserDialogService>()
            ;
    }
}
