using Microsoft.Extensions.DependencyInjection;

namespace BookObserver.ViewModels.Registrator_Locator
{
    internal class ViewModelLocator
    {
        public BooksViewModel BooksVM => App.Services.GetRequiredService<BooksViewModel>();
        public ReadersViewModel ReadersVM => App.Services.GetRequiredService<ReadersViewModel>();
    }
}
