using BookObserver.Models;
using BookObserver.Models.Books;
using BookObserver.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BookObserver.ViewModels
{
    internal class CreatorBookViewModel : ViewModel
    {

        #region Title : string - Заголовок окна

        ///<summary>Заголовок окна</summary>
        private string _title = "Создать книгу";

        ///<summary>Заголовок окна</summary>
        public string Title { get => _title; set => Set(ref _title, value); }

        #endregion

        private readonly BooksViewModel _booksVM;

        public ObservableCollection<Book> Books { get; }

        private readonly CollectionWithFilter _bbksView = new();
        public IList<string>? BBKsView => _bbksView.CollectionView;

        private readonly CollectionWithFilter _authorsView = new();
        public ObservableCollection<string>? AuthorsView => _authorsView.CollectionView;

        private readonly CollectionWithFilter _namesView = new();
        public IList<string>? NamesView => _namesView.CollectionView;

        private readonly CollectionWithFilter _publishesView = new();
        public ObservableCollection<string>? PublishesView => _publishesView.CollectionView;

        private readonly CollectionWithFilter _yearPublishesView = new();
        public ObservableCollection<string>? YearPublishesView => _yearPublishesView.CollectionView;

        #region SelectedCodeAuthor : string? - Выбранный код автора

        ///<summary>Выбранный код автора</summary>
        private string? _selectedCodeAuthor;

        ///<summary>Выбранный код автора</summary>
        public string? SelectedCodeAuthor
        {
            get => _selectedCodeAuthor;
            set => Set(ref _selectedCodeAuthor, value);
        }

        #endregion

        #region SelectedBBK : string? - Выбранный ББК

        ///<summary>Выбранный ББК</summary>
        private string? _selectedBBK;

        ///<summary>Выбранный ББК</summary>
        public string? SelectedBBK
        {
            get => _selectedBBK;
            set
            {
                if (!Set(ref _selectedBBK, value)
                    || value is null) return;

                _bbksView.RefreshFilter(value);
                OnPropertyChanged(nameof(BBKsView));
            }
        }

        #endregion

        #region SelectedAuthor : string? - Выбранный автор

        ///<summary>Выбранный автор</summary>
        private string? _selectedAuthor;

        ///<summary>Выбранный автор</summary>
        public string? SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                if (!Set(ref _selectedAuthor, value)
                    || value is null) return;

                _authorsView.RefreshFilter(value);
                OnPropertyChanged(nameof(AuthorsView));
            }
        }

        #endregion

        #region SelectedName : string? - Выбранное название книги

        ///<summary>Выбранное название книги</summary>
        private string? _selectedName;

        ///<summary>Выбранное название книги</summary>
        public string? SelectedName
        {
            get => _selectedName;
            set
            {
                if (!Set(ref _selectedName, value)
                    || value is null) return;

                _namesView.RefreshFilter(value);
                OnPropertyChanged(nameof(NamesView));
            }
        }

        #endregion

        #region SelectedPublish : string? - Выбранное издательство

        ///<summary>Выбранное издательство</summary>
        private string? _selectedPublish;

        ///<summary>Выбранное издательство</summary>
        public string? SelectedPublish
        {
            get => _selectedPublish;
            set
            {
                if (!Set(ref _selectedPublish, value)
                    || value is null) return;

                _publishesView.RefreshFilter(value);
                OnPropertyChanged(nameof(PublishesView));
            }
        }

        #endregion

        #region SelectedYearPublish : string? - Выбранный год издания

        ///<summary>Выбранный год издания</summary>
        private string? _selectedYearPublish;

        ///<summary>Выбранный год издания</summary>
        public string? SelectedYearPublish
        {
            get => _selectedYearPublish;
            set
            {
                if (!Set(ref _selectedYearPublish, value)
                    || value is null) return;

                _yearPublishesView.RefreshFilter(value);
                OnPropertyChanged(nameof(YearPublishesView));
            }
        }

        #endregion

        #region SelectedPage : string? - Выбранное количество страниц

        ///<summary>Выбранное количество страниц</summary>
        private string? _selectedPages;

        ///<summary>Выбранное количество страниц</summary>
        public string? SelectedPage
        {
            get => _selectedPages;
            set => Set(ref _selectedPages, value);
        }
        #endregion

        #region SelectedISBN : string? - Выбранный ISBN

        ///<summary>Выбранный ISBN</summary>
        private string? _selectedISBN;

        ///<summary>Выбранный ISBN</summary>
        public string? SelectedISBN
        {
            get => _selectedISBN;
            set => Set(ref _selectedISBN, value);
        }

        #endregion

        public CreatorBookViewModel(BooksViewModel booksVM)
        {
            _booksVM = booksVM;
            Books = _booksVM.Books;
            _bbksView.List = Books.Select(b => b.BBK).Distinct().Order().ToList()!;
            _authorsView.List = Books.Select(b => b.Author).Distinct().Order().ToList()!;
            _namesView.List = Books.Select(b => b.Name).Distinct().Order().ToList()!;
            _publishesView.List = Books.Select(b => b.Publish).Distinct().Order().ToList()!;
            _yearPublishesView.List = Books.Select(b => b.YearPublish).Distinct().Order().ToList()!;
        }
    }
}
