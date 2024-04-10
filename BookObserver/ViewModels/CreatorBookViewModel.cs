using BookObserver.Infrastructure.Commands;
using BookObserver.Models;
using BookObserver.Models.Books;
using BookObserver.Services.Interfaces;
using BookObserver.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    internal class CreatorBookViewModel : ViewModel
    {
        private readonly IUserDialog _userDialog;
        private readonly BooksViewModel _booksVM;

        #region Title : string - Заголовок окна

        ///<summary>Заголовок окна</summary>
        private string _title = "Добавить книгу";

        ///<summary>Заголовок окна</summary>
        public string Title { get => _title; set => Set(ref _title, value); }

        #endregion

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

        #region CodeAuthor : string - Код автора

        ///<summary>Код автора</summary>
        private string _codeAuthor = string.Empty;

        ///<summary>Код автора</summary>
        public string CodeAuthor
        {
            get => _codeAuthor;
            set => Set(ref _codeAuthor, value);
        }

        #endregion

        #region BBK : string - ББК

        ///<summary>ББК</summary>
        private string _bbk = string.Empty;

        ///<summary>ББК</summary>
        public string BBK
        {
            get => _bbk;
            set
            {
                if (!Set(ref _bbk, value)) return;

                _bbksView.RefreshFilter(value);
                OnPropertyChanged(nameof(BBKsView));
            }
        }

        #endregion

        #region Author : string - Автор

        ///<summary>Автор</summary>
        private string _author = string.Empty;

        ///<summary>Автор</summary>
        public string Author
        {
            get => _author;
            set
            {
                if (!Set(ref _author, value)) return;

                _authorsView.RefreshFilter(value);
                OnPropertyChanged(nameof(AuthorsView));
            }
        }

        #endregion

        #region Name : string - Название

        ///<summary>Название</summary>
        private string _name = string.Empty;

        ///<summary>Название</summary>
        public string Name
        {
            get => _name;
            set
            {
                if (!Set(ref _name, value)) return;

                _namesView.RefreshFilter(value);
                OnPropertyChanged(nameof(NamesView));
            }
        }

        #endregion

        #region Publish : string - Издательство

        ///<summary>Издательство</summary>
        private string _publish = string.Empty;

        ///<summary>Издательство</summary>
        public string Publish
        {
            get => _publish;
            set
            {
                if (!Set(ref _publish, value)) return;

                _publishesView.RefreshFilter(value);
                OnPropertyChanged(nameof(PublishesView));
            }
        }

        #endregion

        #region YearPublish : string - Год издания

        ///<summary>Год издания</summary>
        private string _yearPublish = string.Empty;

        ///<summary>Год издания</summary>
        public string YearPublish
        {
            get => _yearPublish;
            set
            {
                if (!Set(ref _yearPublish, value)) return;

                _yearPublishesView.RefreshFilter(value);
                OnPropertyChanged(nameof(YearPublishesView));
            }
        }

        #endregion

        #region Pages : string - Количество страниц

        ///<summary>Количество страниц</summary>
        private string _pages = string.Empty;

        ///<summary>Количество страниц</summary>
        public string Pages
        {
            get => _pages;
            set => Set(ref _pages, value);
        }
        #endregion

        #region ISBN : string - ISBN

        ///<summary>ISBN</summary>
        private string _isbn = string.Empty;

        ///<summary>ISBN</summary>
        public string ISBN
        {
            get => _isbn;
            set => Set(ref _isbn, value);
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
            !string.IsNullOrWhiteSpace(_codeAuthor)
            || !string.IsNullOrWhiteSpace(_bbk)
            || !string.IsNullOrWhiteSpace(_author)
            || !string.IsNullOrWhiteSpace(_name)
            || !string.IsNullOrWhiteSpace(_publish)
            || !string.IsNullOrWhiteSpace(_yearPublish)
            || !string.IsNullOrWhiteSpace(_pages)
            || !string.IsNullOrWhiteSpace(_isbn)
            ;

        ///<summary>Логика выполнения - COMMENT</summary>
        private void OnClearFieldsCommandExecuted(object? p)
        {
            CodeAuthor = string.Empty;
            BBK = string.Empty;
            Author = string.Empty;
            Name = string.Empty;
            Publish = string.Empty;
            YearPublish = string.Empty;
            Pages = string.Empty;
            ISBN = string.Empty;
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
            Books.Add(new Book
            {
                Id = Books.Count,
                Existence = "Да",
                CodeAuthor = _codeAuthor,
                BBK = _bbk,
                Author = _author,
                Name = _name,
                Publish = _publish,
                YearPublish = _yearPublish,
                Pages = _pages,
                ISBN = _isbn
            });
            _booksVM._booksView.View.Refresh();
            if (_isNotifyAddBook)
                _userDialog.ShowInformation("Книга успешно добавлена в список", _title);
        }

        #endregion

        #endregion

        public CreatorBookViewModel(BooksViewModel booksVM, IUserDialog userDialog)
        {
            _userDialog = userDialog;
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
