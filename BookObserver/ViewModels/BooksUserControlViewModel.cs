using BookObserver.Models.Books;
using BookObserver.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace BookObserver.ViewModels
{
    class BooksUserControlViewModel : ViewModel
    {
        #region Books : ObservableCollection<Book> - Список книг

        ///<summary>Список книг</summary>
        private ObservableCollection<Book>? _books;

        ///<summary>Список книг</summary>
        public ObservableCollection<Book>? Books
        {
            get => _books;
            set
            {
                if (!Set(ref _books, value)) return;

                _booksView.Source = value;
                OnPropertyChanged(nameof(BooksView));
            }
        }

        #endregion

        private readonly CollectionViewSource _booksView = new();
        public ICollectionView BooksView => _booksView.View;

        #region BooksFilterText : string? - фильтр книг

        ///<summary>фильтр книг</summary>
        private string? _booksFilterText;

        ///<summary>фильтр книг</summary>
        public string? BooksFilterText
        {
            get => _booksFilterText;
            set
            {
                if(!Set(ref _booksFilterText, value)) return;

                _booksView.View.Refresh();
            }
        }

        #endregion


        public BooksUserControlViewModel()
        {
            Books = new(Enumerable.Range(0, 100000).Select(p => new Book
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
            _booksView.Filter += BooksView_Filter;
        }

        private void BooksView_Filter(object sender, FilterEventArgs e)
        {
            if(e.Item is not Book book)
            {
                e.Accepted = false;
                return;
            }

            var filter_text = _booksFilterText;
            if (string.IsNullOrWhiteSpace(filter_text)) return;

            if (book.BBK.Contains(filter_text, StringComparison.OrdinalIgnoreCase)) return;
            if (book.Author.Contains(filter_text, StringComparison.OrdinalIgnoreCase)) return;
            if (book.Name.Contains(filter_text, StringComparison.OrdinalIgnoreCase)) return;

            e.Accepted = false;
        }
    }
}
