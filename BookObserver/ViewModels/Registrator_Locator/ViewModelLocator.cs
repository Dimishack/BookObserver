using Microsoft.Extensions.DependencyInjection;

namespace BookObserver.ViewModels.Registrator_Locator
{
    internal class ViewModelLocator
    {
        public BooksUserControlViewModel BooksUC => App.Services.GetRequiredService<BooksUserControlViewModel>();
    }
}
