using BookObserver.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookObserver.Services.Registrator
{
    internal static class ServicesRegistrator
    {
        public static IServiceCollection AddServices(this IServiceCollection services) =>
            services.AddSingleton<IUserDialog, UserDialogService>()
            ;
    }
}
