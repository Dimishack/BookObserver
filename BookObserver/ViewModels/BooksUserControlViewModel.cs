using BookObserver.Models.Books;
using BookObserver.ViewModels.Base;
using System;
using System.Collections.Immutable;
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
                _authorsView.Source = value?.Select(p => p.Author).ToImmutableSortedSet();
                _namesView.Source = value?.Select(p => p.Name).ToImmutableSortedSet();
            }
        }

        #endregion

        #region BooksView : ICollectionView - Вывод списка книг

        private readonly CollectionViewSource _booksView = new();
        public ICollectionView BooksView => _booksView.View;

        #endregion

        #region BooksFilterText : string? - фильтр книг

        ///<summary>фильтр книг</summary>
        private string? _booksFilterText;

        ///<summary>фильтр книг</summary>
        public string? BooksFilterText
        {
            get => _booksFilterText;
            set
            {
                if (!Set(ref _booksFilterText, value)) return;

                _booksView.View.Refresh();
            }
        }

        #endregion

        #region AuthorsView : ICollectionView - Вывод списка авторов

        private readonly CollectionViewSource _authorsView = new();
        public ICollectionView AuthorsView => _authorsView.View;

        #endregion

        #region AuthorsFilterText : string? - фильтр авторов

        ///<summary>фильтр авторов</summary>
        private string? _authorsFilterText;

        ///<summary>фильтр авторов</summary>
        public string? AuthorsFilterText
        {
            get => _authorsFilterText;
            set
            {
                if (!Set(ref _authorsFilterText, value)) return;

                _authorsView.View.Refresh();
            }
        }

        #endregion

        #region NamesView : ICollectionView - Вывод списка названий книг
        private readonly CollectionViewSource _namesView = new();
        public ICollectionView NamesView => _namesView.View; 
        #endregion

        #region NameFilterText : string? - фильтр названий

        ///<summary>фильтр названий</summary>
        private string? _nameFilterText;

        ///<summary>фильтр названий</summary>
        public string? NameFilterText
        {
            get => _nameFilterText;
            set
            {
                if (!Set(ref _nameFilterText, value)) return;

                _namesView.View.Refresh();
            }
        }

        #endregion


        public BooksUserControlViewModel()
        {
            Books = new(Enumerable.Range(0, 50000).Select(p => new Book
            {
                Id = p,
                BBK = $"{p}{p}",
                Pages = p + Random.Shared.Next(0, 100),
                Author = $"Author {p}",
                Name = new string('ü', Random.Shared.Next(15,30)),
                Reader = new Models.Readers.Reader
                {
                    FirstName = "Амплитуда"
                }
            }));
            _booksView.Filter += BooksView_Filter;
            _authorsView.Filter += AuthorsView_Filter;
            _namesView.Filter += NamesView_Filter;
        }

        #region Events

        private void NamesView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not string name)
            {
                e.Accepted = false;
                return;
            }
            var filter_text = _nameFilterText;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text) || name.Contains(filter_text);
        }

        private void AuthorsView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not string author)
            {
                e.Accepted = false;
                return;
            }
            var filter_text = _authorsFilterText;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text) || author.Contains(filter_text);
        }

        private void BooksView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not Book book)
            {
                e.Accepted = false;
                return;
            }
            var filter_text = _booksFilterText;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text) ||
                book.BBK.Contains(filter_text, StringComparison.OrdinalIgnoreCase) ||
                book.Author.Contains(filter_text, StringComparison.OrdinalIgnoreCase) ||
                book.Name.Contains(filter_text, StringComparison.OrdinalIgnoreCase) ||
                book.Publish.Contains(filter_text, StringComparison.OrdinalIgnoreCase) ||
                book.YearPublish.ToString().Contains(filter_text, StringComparison.OrdinalIgnoreCase) ||
                book.CodeAuthor.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                ;
        } 

        #endregion
    }
}
