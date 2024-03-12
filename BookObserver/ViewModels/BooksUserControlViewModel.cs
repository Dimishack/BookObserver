using BookObserver.Models.Books;
using BookObserver.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace BookObserver.ViewModels
{
    class BooksUserControlViewModel : ViewModel
    {
        public ObservableCollection<Book> Books { get; }

        public BooksUserControlViewModel()
        {
            Books = new ObservableCollection<Book>(Enumerable.Range(0, 1000000).Select(p => new Book
            {
                Id = p,
                BBK = $"{p}{p}",
                Pages = p + Random.Shared.Next(0, 100),
                Author = $"Author {p}",
                Name = new string('ü', 150),
                Reader = new Models.Readers.Reader
                {
                    FirstName = "Амплитуда"
                }
            }));
        }
    }
}
