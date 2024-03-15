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
                _stockView.Source = value?.Select(p => p.Stock).ToImmutableSortedSet();
                _bbkView.Source = value?.Select(p => p.BBK).ToImmutableSortedSet();
                _authorsView.Source = value?.Select(p => p.Author).ToImmutableSortedSet();
                _namesView.Source = value?.Select(p => p.Name).ToImmutableSortedSet();

            }
        }

        #endregion

        #region BooksView : ICollectionView - Вывод списка книг

        private readonly CollectionViewSource _booksView = new();
        public ICollectionView BooksView => _booksView.View;

        #endregion

        #region BooksFilterText : string? - Фильтр книг

        ///<summary>Фильтр книг</summary>
        private string? _booksFilterText;

        ///<summary>Фильтр книг</summary>
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

        #region StockView : ICollectionView - Вывод списка "в наличии"
        private readonly CollectionViewSource _stockView = new();
        public ICollectionView StockView => _stockView.View;
        #endregion

        #region StockFilterText : string? - Фильтр "в наличии"

        ///<summary>Фильтр "в наличии"</summary>
        private string? _stockFilterText;

        ///<summary>Фильтр "в наличии"</summary>
        public string? StockFilterText
        {
            get => _stockFilterText;
            set
            {
                if (!Set(ref _stockFilterText, value)) return;

                _stockView.View.Refresh();
            }
        }

        #endregion

        #region AuthorsView : ICollectionView - Вывод списка авторов

        private readonly CollectionViewSource _authorsView = new();
        public ICollectionView AuthorsView => _authorsView.View;

        #endregion

        #region AuthorsFilterText : string? - Фильтр авторов

        ///<summary>Фильтр авторов</summary>
        private string? _authorsFilterText;

        ///<summary>Фильтр авторов</summary>
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

        #region NameFilterText : string? - Фильтр названий

        ///<summary>Фильтр названий</summary>
        private string? _nameFilterText;

        ///<summary>Фильтр названий</summary>
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

        #region BBKView : ICollectionView - Вывод списка BBK
        private readonly CollectionViewSource _bbkView = new();
        public ICollectionView BBKView => _bbkView.View; 
        #endregion

        #region BBKFilterText : string? - Фильтр ББК

        ///<summary>Фильтр ББК</summary>
        private string? _bbkFilterText;

        ///<summary>Фильтр ББК</summary>
        public string? BBKFilterText
        {
            get => _bbkFilterText;
            set
            {
                if (!Set(ref _bbkFilterText, value)) return;

                _bbkView.View.Refresh();
            }
        }

        #endregion

        public BooksUserControlViewModel()
        {
            Books = new(Enumerable.Range(0, 50000).Select(p => new Book
            {
                Id = p,
                BBK = Random.Shared.Next(0, 100).ToString(),
                Pages = p + Random.Shared.Next(0, 100),
                Author = $"Author {p}",
                Name = new string('ü', Random.Shared.Next(15, 30)),
                Reader = new Models.Readers.Reader
                {
                    FirstName = "Амплитуда"
                },
                Stock = Random.Shared.Next(0, 2) == 0
            }));
            _stockView.Filter += StockView_Filter;
            _bbkView.Filter += BBKView_Filter;
            _authorsView.Filter += AuthorsView_Filter;
            _namesView.Filter += NamesView_Filter;
            _booksView.Filter += BooksView_Filter;
        }


        #region Events

        #region BBKView_Filter
        private void BBKView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not string bbk)
            {
                e.Accepted = false;
                return;
            }
            var filter_text = _bbkFilterText;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || bbk.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                ;
        } 
        #endregion

        #region StockView_Filter
        private void StockView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not bool stock)
            {
                e.Accepted = false;
                return;
            }
            var stock_string = stock ? "да" : "нет";
            var filter_text = _stockFilterText;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || stock_string.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                ;
        }
        #endregion

        #region StockViewFilter
        private void NamesView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not string name)
            {
                e.Accepted = false;
                return;
            }
            var filter_text = _nameFilterText;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || name.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                ;
        }
        #endregion

        #region AuthorView_Filter
        private void AuthorsView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not string author)
            {
                e.Accepted = false;
                return;
            }
            var filter_text = _authorsFilterText;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || author.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                ;
        }
        #endregion

        #region BooksView_Filter
        private void BooksView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not Book book)
            {
                e.Accepted = false;
                return;
            }
            var filter_text = _booksFilterText;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || book.BBK.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                || book.Author.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                || book.Name.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                || book.Publish.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                || book.YearPublish.ToString().Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                || book.CodeAuthor.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                ;
        }
        #endregion

        #endregion
    }
}
