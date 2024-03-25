using BookObserver.Infrastructure.Commands;
using BookObserver.Models.Books;
using BookObserver.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    internal class Creator_EditorBookViewModel : ViewModel
    {
        private readonly BooksUserControlViewModel _booksViewModel;

        private ObservableCollection<Book> Books { get; }

        #region Title : string - Заголовок окна

        ///<summary>Заголовок окна</summary>
        private string _title = "Создать книгу";

        ///<summary>Заголовок окна</summary>
        public string Title { get => _title; set => Set(ref _title, value); }

        #endregion

        #region SelectedCodeAuthor : string? - Выбранный код автора

        ///<summary>Выбранный код автора</summary>
        private string? _selectedCodeAuthor;

        ///<summary>Выбранный код автора</summary>
        public string? SelectedCodeAuthor { get => _selectedCodeAuthor; set => Set(ref _selectedCodeAuthor, value); }

        #endregion

        #region SelectedBBK : double - Выбранный ББК

        ///<summary>Выбранный ББК</summary>
        private double _selectedBBK;

        ///<summary>Выбранный ББК</summary>
        public double SelectedBBK { get => _selectedBBK; set => Set(ref _selectedBBK, value); }

        #endregion

        #region SelectedAuthor : string? - Выбранный автор

        ///<summary>Выбранный автор</summary>
        private string? _selectedAuthor;

        ///<summary>Выбранный автор</summary>
        public string? SelectedAuthor { get => _selectedAuthor; set => Set(ref _selectedAuthor, value); }

        #endregion

        #region SelectedName : string? - Выбранное название

        ///<summary>Выбранное название</summary>
        private string? _selectedName;

        ///<summary>Выбранное название</summary>
        public string? SelectedName { get => _selectedName; set => Set(ref _selectedName, value); }

        #endregion

        #region SelectedPublish : string? - Выбранное издательство

        ///<summary>Выбранное издательство</summary>
        private string? _selectedPublish;

        ///<summary>Выбранное издательство</summary>
        public string? SelectedPublish { get => _selectedPublish; set => Set(ref _selectedPublish, value); }

        #endregion

        #region SelectedYearPublish : int - Выбранный год издания

        ///<summary>Выбранный год издания</summary>
        private int _selectedYearPublish;

        ///<summary>Выбранный год издания</summary>
        public int SelectedYearPublish { get => _selectedYearPublish; set => Set(ref _selectedYearPublish, value); }

        #endregion

        #region SelectedPages : int - Выбранное количество страниц

        ///<summary>Выбранное количество страниц</summary>
        private int _selectedPages;

        ///<summary>Выбранное количество страниц</summary>
        public int SelectedPages { get => _selectedPages; set => Set(ref _selectedPages, value); }

        #endregion

        #region SelectedISBN : string? - Выбранный ISBN

        ///<summary>Выбранный ISBN</summary>
        private string? _selectedISBN = string.Empty;

        ///<summary>Выбранный ISBN</summary>
        public string? SelectedISBN { get => _selectedISBN; set => Set(ref _selectedISBN, value); }

        #endregion

        #region Stock : bool - В наличии

        ///<summary>В наличии</summary>
        private bool _stock = true;

        ///<summary>В наличии</summary>
        public bool Stock { get => _stock; set => Set(ref _stock, value); }

        #endregion

        #region SelectedFirstName : string? - Выбранное имя

        ///<summary>Выбранное имя</summary>
        private string? _selectedFirstName;

        ///<summary>Выбранное имя</summary>
        public string? SelectedFirstName { get => _selectedFirstName; set => Set(ref _selectedFirstName, value); }

        #endregion

        #region SelectedLastName : string? - Выбранная фамилия

        ///<summary>Выбранная фамилия</summary>
        private string? _selectedLastName;

        ///<summary>Выбранная фамилия</summary>
        public string? SelectedLastName { get => _selectedLastName; set => Set(ref _selectedLastName, value); }

        #endregion

        #region SelectedPatronymic : string? - Выбранное отчество

        ///<summary>Выбранное отчество</summary>
        private string? _selectedPatronymic;

        ///<summary>Выбранное отчество</summary>
        public string? SelectedPatronymic { get => _selectedPatronymic; set => Set(ref _selectedPatronymic, value); }

        #endregion

        #region DateGetBook : DateTime - Дата получения книги

        ///<summary>Дата получения книги</summary>
        private DateTime _dateGetBook;

        ///<summary>Дата получения книги</summary>
        public DateTime DateGetBook { get => _dateGetBook; set => Set(ref _dateGetBook, value); }

        #endregion

        #region DateSetBook : DateTime - Дата возврата книги

        ///<summary>Дата возврата книги</summary>
        private DateTime _dateSetBook;

        ///<summary>Дата возврата книги</summary>
        public DateTime DateSetBook { get => _dateSetBook; set => Set(ref _dateSetBook, value); }

        #endregion

        #region NotifyAddBook : bool - Уведомление о добавлении книги

        ///<summary>Уведомление о добавлении книги</summary>
        private bool _notifyAddBook = false;

        ///<summary>Уведомление о добавлении книги</summary>
        public bool NotifyAddBook { get => _notifyAddBook; set => Set(ref _notifyAddBook, value); }

        #endregion

        #region Commands

        #region CloseWindowCommand - Команда закрытия окна

        ///<summary>Команда закрытия окна</summary>
        private ICommand? _closewindowCommand;

        ///<summary>Команда закрытия окна</summary>
        public ICommand CloseWindowCommand => _closewindowCommand
            ??= new LambdaCommand(OnCloseWindowCommandExecuted);

        ///<summary>Логика выполнения - Команда закрытия окна</summary>
        private void OnCloseWindowCommandExecuted(object? p) => App.ActiveWindow.Close();

        #endregion

        #region AddBookCommand - Команда добавления книги

        ///<summary>Команда добавления книги</summary>
        private ICommand? _addBookCommand;

        ///<summary>Команда добавления книги</summary>
        public ICommand AddBookCommand => _addBookCommand
            ??= new LambdaCommand(OnAddBookCommandExecuted, CanAddBookCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда добавления книги</summary>
        private bool CanAddBookCommandExecute(object? p) => 
            !string.IsNullOrWhiteSpace(_selectedCodeAuthor)
            && _selectedBBK > 0.0
            && !string.IsNullOrWhiteSpace(_selectedAuthor)
            && !string.IsNullOrWhiteSpace(_selectedName)
            && !string.IsNullOrWhiteSpace(_selectedPublish)
            && _selectedYearPublish > 0
            && _selectedPages > 0
            ;

        ///<summary>Логика выполнения - Команда добавления книги</summary>
        private void OnAddBookCommandExecuted(object? p)
        {
            var book = new Book
            {
                Id = Books.Count,
                Stock = this.Stock ? "Да" : "Нет",
                CodeAuthor = _selectedCodeAuthor,
                BBK = Convert.ToString(_selectedBBK),
                Author = _selectedAuthor,
                Name = _selectedName,
                Publish = _selectedPublish,
                YearPublish = Convert.ToString(_selectedYearPublish),
                Pages = _selectedPages,
                ISBN = _selectedISBN
            };
            _booksViewModel?.Books?.Add(book);
            _booksViewModel?._booksView.View.Refresh();
        }

        #endregion

        #region ClearFieldsForBookCommand - Команда очистки полей информации о книге

        ///<summary>Команда очистки полей информации о книге</summary>
        private ICommand? _clearFieldsForBookCommand;

        ///<summary>Команда очистки полей информации о книге</summary>
        public ICommand ClearFieldsForBookCommand => _clearFieldsForBookCommand
            ??= new LambdaCommand(OnClearFieldsForBookCommandExecuted, CanClearFieldsForBookCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда очистки полей информации о книге</summary>
        private bool CanClearFieldsForBookCommandExecute(object? p) => 
            _selectedCodeAuthor is not null
            || _selectedBBK > 0.0
            || _selectedAuthor is not null
            || _selectedName is not null
            || _selectedPublish is not null
            || _selectedYearPublish > 0
            || _selectedPages > 0
            || _selectedISBN is not null
            ;

        ///<summary>Логика выполнения - Команда очистки полей информации о книге</summary>
        private void OnClearFieldsForBookCommandExecuted(object? p)
        {
            SelectedCodeAuthor = default;
            SelectedBBK = default;
            SelectedAuthor = default;
            SelectedName = default;
            SelectedPublish = default;
            SelectedYearPublish = default;
            SelectedPages = default;
            SelectedISBN = default;
        }

        #endregion

        #endregion

        public Creator_EditorBookViewModel(BooksUserControlViewModel booksViewModel)
        {
            _booksViewModel = booksViewModel;

            Books = new ObservableCollection<Book>(_booksViewModel.Books);
        }

    }
}
