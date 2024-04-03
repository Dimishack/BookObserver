using BookObserver.Infrastructure.Commands;
using BookObserver.Models;
using BookObserver.Models.Books;
using BookObserver.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    internal class CreatorBookViewModel : ViewModel
    {
        #region Title : string - Заголовок окна

        ///<summary>Заголовок окна</summary>
        private string _title = "Добавить книгу";

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
        private string _selectedCodeAuthor = string.Empty;

        ///<summary>Выбранный код автора</summary>
        public string SelectedCodeAuthor
        {
            get => _selectedCodeAuthor;
            set => Set(ref _selectedCodeAuthor, value);
        }

        #endregion

        #region SelectedBBK : string? - Выбранный ББК

        ///<summary>Выбранный ББК</summary>
        private string _selectedBBK = string.Empty;

        ///<summary>Выбранный ББК</summary>
        public string SelectedBBK
        {
            get => _selectedBBK;
            set
            {
                if (!Set(ref _selectedBBK, value)) return;

                _bbksView.RefreshFilter(value);
                OnPropertyChanged(nameof(BBKsView));
            }
        }

        #endregion

        #region SelectedAuthor : string? - Выбранный автор

        ///<summary>Выбранный автор</summary>
        private string _selectedAuthor = string.Empty;

        ///<summary>Выбранный автор</summary>
        public string SelectedAuthor
        {
            get => _selectedAuthor;
            set
            {
                if (!Set(ref _selectedAuthor, value)) return;

                _authorsView.RefreshFilter(value);
                OnPropertyChanged(nameof(AuthorsView));
            }
        }

        #endregion

        #region SelectedName : string? - Выбранное название книги

        ///<summary>Выбранное название книги</summary>
        private string _selectedName = string.Empty;

        ///<summary>Выбранное название книги</summary>
        public string SelectedName
        {
            get => _selectedName;
            set
            {
                if (!Set(ref _selectedName, value)) return;

                _namesView.RefreshFilter(value);
                OnPropertyChanged(nameof(NamesView));
            }
        }

        #endregion

        #region SelectedPublish : string? - Выбранное издательство

        ///<summary>Выбранное издательство</summary>
        private string _selectedPublish = string.Empty;

        ///<summary>Выбранное издательство</summary>
        public string SelectedPublish
        {
            get => _selectedPublish;
            set
            {
                if (!Set(ref _selectedPublish, value)) return;

                _publishesView.RefreshFilter(value);
                OnPropertyChanged(nameof(PublishesView));
            }
        }

        #endregion

        #region SelectedYearPublish : string? - Выбранный год издания

        ///<summary>Выбранный год издания</summary>
        private string _selectedYearPublish = string.Empty;

        ///<summary>Выбранный год издания</summary>
        public string SelectedYearPublish
        {
            get => _selectedYearPublish;
            set
            {
                if (!Set(ref _selectedYearPublish, value)) return;

                _yearPublishesView.RefreshFilter(value);
                OnPropertyChanged(nameof(YearPublishesView));
            }
        }

        #endregion

        #region SelectedPage : string? - Выбранное количество страниц

        ///<summary>Выбранное количество страниц</summary>
        private string _selectedPages = string.Empty;

        ///<summary>Выбранное количество страниц</summary>
        public string SelectedPage
        {
            get => _selectedPages;
            set => Set(ref _selectedPages, value);
        }
        #endregion

        #region SelectedISBN : string? - Выбранный ISBN

        ///<summary>Выбранный ISBN</summary>
        private string _selectedISBN = string.Empty;

        ///<summary>Выбранный ISBN</summary>
        public string SelectedISBN
        {
            get => _selectedISBN;
            set => Set(ref _selectedISBN, value);
        }

        #endregion

        #region IsNotifyAddBook : bool - Разрешение уведомлять о добавлении книги

        ///<summary>Разрешение уведомлять о добавлении книги</summary>
        private bool _isNotifyAddBook;

        ///<summary>Разрешение уведомлять о добавлении книги</summary>
        public bool IsNotifyAddBook { get => _isNotifyAddBook; set => Set(ref _isNotifyAddBook, value); }

        #endregion

        #region Commands...

        #region ClearFieldsCommand - Команда очистки полей

        ///<summary>COMMENT</summary>
        private ICommand? _clearFieldsCommand;

        ///<summary>COMMENT</summary>
        public ICommand ClearFieldsCommand => _clearFieldsCommand
            ??= new LambdaCommand(OnClearFieldsCommandExecuted, CanClearFieldsCommandExecute);

        ///<summary>Проверка возможности выполнения - COMMENT</summary>
        private bool CanClearFieldsCommandExecute(object? p) => 
            !string.IsNullOrWhiteSpace(_selectedCodeAuthor)
            || !string.IsNullOrWhiteSpace(_selectedBBK)
            || !string.IsNullOrWhiteSpace(_selectedAuthor)
            || !string.IsNullOrWhiteSpace(_selectedName)
            || !string.IsNullOrWhiteSpace(_selectedPublish)
            || !string.IsNullOrWhiteSpace(_selectedYearPublish)
            || !string.IsNullOrWhiteSpace(_selectedPages)
            || !string.IsNullOrWhiteSpace(_selectedISBN)
            ;

        ///<summary>Логика выполнения - COMMENT</summary>
        private void OnClearFieldsCommandExecuted(object? p)
        {
            SelectedCodeAuthor = string.Empty;
            SelectedBBK = string.Empty;
            SelectedAuthor = string.Empty;
            SelectedName = string.Empty;
            SelectedPublish = string.Empty;
            SelectedYearPublish = string.Empty;
            SelectedPage = string.Empty;
            SelectedISBN = string.Empty;
        }

        #endregion

        #region CloseWindowCommand - Команда закрытия окна

        ///<summary>Команда закрытия окна</summary>
        private ICommand? _closeWindowCommand;

        ///<summary>Команда закрытия окна</summary>
        public ICommand CloseWindowCommand => _closeWindowCommand
            ??= new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда закрытия окна</summary>
        private bool CanCloseWindowCommandExecute(object? p) => p is Window;

        ///<summary>Логика выполнения - Команда закрытия окна</summary>
        private void OnCloseWindowCommandExecuted(object? p) => (p as Window)!.Close();

        #endregion

        #region AddBookCommand - Команда добавления книги в основной список

        ///<summary>Команда добавления книги в основной список</summary>
        private ICommand? _addBookCommand;

        ///<summary>Команда добавления книги в основной список</summary>
        public ICommand AddBookCommand => _addBookCommand
            ??= new LambdaCommand(OnAddBookCommandExecuted, CanAddBookCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда добавления книги в основной список</summary>
        private bool CanAddBookCommandExecute(object? p) => true;

        ///<summary>Логика выполнения - Команда добавления книги в основной список</summary>
        private void OnAddBookCommandExecuted(object? p)
        {
            var book = new Book
            {
                Id = Books.Count,
                Existence = "Да",
                CodeAuthor = _selectedCodeAuthor,
                BBK = _selectedBBK,
                Author = _selectedAuthor,
                Name = _selectedName,
                Publish = _selectedPublish,
                YearPublish = _selectedYearPublish,
                Pages = _selectedPages,
                ISBN = _selectedISBN
            };
            Books.Add(book);
            _booksVM._booksView.View.Refresh();
            if (_isNotifyAddBook)
                MessageBox.Show("Книга успешно добавлена в список", _title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

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
