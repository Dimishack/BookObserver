using BookObserver.Models.Books;
using BookObserver.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace BookObserver.ViewModels
{
    class BooksUserControlViewModel : ViewModel
    {
        public ObservableCollection<Book> Books { get; }

        public IList<int?> ListPages
        {
            get
            {
                List<int?> list = [];
                foreach (var book in Books)
                    if(!list.Contains(book.Pages))
                        list.Add(book.Pages);
                list.Sort();
                return list;
            }
        }

        public BooksUserControlViewModel()
        {
            Books = new (Enumerable.Range(0, 10000).Select(p => new Book
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
