using BookObserver.Infrastructure.Commands;
using BookObserver.Models.Books;
using BookObserver.ViewModels.Base;
using BookObserver.Views.Windows;
using System.Windows;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    class EditorBookViewModel : ViewModel
    {
        private readonly BooksViewModel _booksVM;
        private readonly ReadersViewModel _readersVM;
        private readonly Book _bookOnEdit;
        private readonly int _indexBook;

        #region Existence : bool - В наличии

        ///<summary>В наличии</summary>
        private bool _existence;

        ///<summary>В наличии</summary>
        public bool Existence { get => _existence; set => Set(ref _existence, value); }

        #endregion

        #region SelectedCodeAuthor : string - Выбранный код автора

        ///<summary>Выбранный код автора</summary>
        private string _selectedCodeAuthor = string.Empty;

        ///<summary>Выбранный код автора</summary>
        public string SelectedCodeAuthor { get => _selectedCodeAuthor; set => Set(ref _selectedCodeAuthor, value); }

        #endregion

        #region SelectedBBK : string - Выбранный ББК

        ///<summary>Выбранный ББК</summary>
        private string _selectedBBK = string.Empty;

        ///<summary>Выбранный ББК</summary>
        public string SelectedBBK { get => _selectedBBK; set => Set(ref _selectedBBK, value); }

        #endregion

        #region SelectedAuthor : string - Выбранный автор

        ///<summary>Выбранный автор</summary>
        private string _selectedAuthor = string.Empty;

        ///<summary>Выбранный автор</summary>
        public string SelectedAuthor { get => _selectedAuthor; set => Set(ref _selectedAuthor, value); }

        #endregion

        #region SelectedName : string - Выбранное название

        ///<summary>Выбранное название</summary>
        private string _selectedName = string.Empty;

        ///<summary>Выбранное название</summary>
        public string SelectedName { get => _selectedName; set => Set(ref _selectedName, value); }

        #endregion

        #region SelectedPublish : string - Выбранное издательство

        ///<summary>Выбранное издательство</summary>
        private string _selectedPublish = string.Empty;

        ///<summary>Выбранное издательство</summary>
        public string SelectedPublish { get => _selectedPublish; set => Set(ref _selectedPublish, value); }

        #endregion

        #region SelectedYearPublish : string - Выбранный год издания

        ///<summary>Выбранный год издания</summary>
        private string _selectedYearPublish = string.Empty;

        ///<summary>Выбранный год издания</summary>
        public string SelectedYearPublish { get => _selectedYearPublish; set => Set(ref _selectedYearPublish, value); }

        #endregion

        #region SelectedPage : string - Выбранное количество страниц

        ///<summary>Выбранное количество страниц</summary>
        private string _selectedPage = string.Empty;

        ///<summary>Выбранное количество страниц</summary>
        public string SelectedPage { get => _selectedPage; set => Set(ref _selectedPage, value); }

        #endregion

        #region SelectedISBN : string - Выбранный ISBN

        ///<summary>Выбранный ISBN</summary>
        private string _selectedISBN = string.Empty;

        ///<summary>Выбранный ISBN</summary>
        public string SelectedISBN { get => _selectedISBN; set => Set(ref _selectedISBN, value); }

        #endregion

        #region IdReader : int? - Id выбранного читателя

        ///<summary>Id выбранного читателя</summary>
        private int? _idReader;

        ///<summary>Id выбранного читателя</summary>
        public int? IdReader { get => _idReader; set => Set(ref _idReader, value); }

        #endregion

        #region FullNameReader : string? - ФИО читателя

        ///<summary>ФИО читателя</summary>
        private string? _fullNameReader;

        ///<summary>ФИО читателя</summary>
        public string? FullNameReader { get => _fullNameReader; set => Set(ref _fullNameReader, value); }

        #endregion

        #region SelectedDateGet : DateTime? - Выбранная дата получения

        ///<summary>Выбранная дата получения</summary>
        private DateTime? _selectedDateGet;

        ///<summary>Выбранная дата получения</summary>
        public DateTime? SelectedDateGet
        {
            get => _selectedDateGet;
            set
            {
                if (!Set(ref _selectedDateGet, value)) return;

                if (value is not null)
                    SelectedDateSet = value.Value.AddMonths(1);
            }
        }

        #endregion

        #region SelectedDateSet : DateTime? - Выбранная дата возврата

        ///<summary>Выбранная дата возврата</summary>
        private DateTime? _selectedDateSet;

        ///<summary>Выбранная дата возврата</summary>
        public DateTime? SelectedDateSet { get => _selectedDateSet; set => Set(ref _selectedDateSet, value); }

        #endregion

        #region Commands...

        #region ResetCommand - Команда возвращения в первоначальный вид

        ///<summary>Команда возвращения в первоначальный вид</summary>
        private ICommand? _resetCommand;

        ///<summary>Команда возвращения в первоначальный вид</summary>
        public ICommand ResetCommand => _resetCommand
            ??= new LambdaCommand(OnResetCommandExecuted, CanResetCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда возвращения в первоначальный вид</summary>
        private bool CanResetCommandExecute(object? p) =>
            _selectedCodeAuthor != _bookOnEdit.CodeAuthor
            || _selectedBBK != _bookOnEdit.BBK
            || _selectedAuthor != _bookOnEdit.Author
            || _selectedName != _bookOnEdit.Name
            || _selectedPublish != _bookOnEdit.Publish
            || _selectedYearPublish != _bookOnEdit.YearPublish
            || _selectedPage != _bookOnEdit.Pages
            || _selectedISBN != _bookOnEdit.ISBN
            ;

        ///<summary>Логика выполнения - Команда возвращения в первоначальный вид</summary>
        private void OnResetCommandExecuted(object? p)
        {
            SelectedCodeAuthor = _bookOnEdit.CodeAuthor;
            SelectedBBK = _bookOnEdit.BBK;
            SelectedAuthor = _bookOnEdit.Author;
            SelectedName = _bookOnEdit.Name;
            SelectedPublish = _bookOnEdit.Publish;
            SelectedYearPublish = _bookOnEdit.YearPublish;
            SelectedPage = _bookOnEdit.Pages;
            SelectedISBN = _bookOnEdit.ISBN;
            Existence = _bookOnEdit.Existence == "Да";
        }

        #endregion

        #region EditBookCommand - Редактировать книгу (нажатие на ОК)

        ///<summary>Редактировать книгу (нажатие на ОК)</summary>
        private ICommand? _editBookCommand;

        ///<summary>Редактировать книгу (нажатие на ОК)</summary>
        public ICommand EditBookCommand => _editBookCommand
            ??= new LambdaCommand(OnEditBookCommandExecuted, CanEditBookCommandExecute);

        ///<summary>Проверка возможности выполнения - Редактировать книгу (нажатие на ОК)</summary>
        private bool CanEditBookCommandExecute(object? p) =>
            p is Window
            && (
            _existence
            || (!string.IsNullOrWhiteSpace(_fullNameReader)
            && _idReader is not null
            && _selectedDateGet is not null
            && _selectedDateSet is not null
            ))
            ;

        ///<summary>Логика выполнения - Редактировать книгу (нажатие на ОК)</summary>
        private void OnEditBookCommandExecuted(object? p)
        {
            _booksVM.Books[_indexBook] = new Book
            {
                Id = _bookOnEdit.Id,
                IdReader = _idReader,
                CodeAuthor = _selectedCodeAuthor,
                BBK = _selectedBBK,
                Author = _selectedAuthor,
                Name = _selectedName,
                Publish = _selectedPublish,
                YearPublish = _selectedYearPublish,
                Pages = _selectedPage,
                ISBN = _selectedISBN,
                Existence = _existence ? "Да" : "Нет",
                FullNameReader = _fullNameReader,
                DateGet = _selectedDateGet,
                DateSet = _selectedDateSet
            };
            if (_idReader is not null)
                _readersVM.Readers[(int)_idReader].ListIdBook.Add(_indexBook);
            (p as Window)!.Close();
        }

        #endregion

        #region CancelEditCommand - Команда отмены редактирования

        ///<summary>Команда отмены редактирования</summary>
        private ICommand? _cancelEditCommand;

        ///<summary>Команда отмены редактирования</summary>
        public ICommand CancelEditCommand => _cancelEditCommand
            ??= new LambdaCommand(OnCancelEditCommandExecuted, CanCancelEditCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда отмены редактирования</summary>
        private bool CanCancelEditCommandExecute(object? p) => p is Window;

        ///<summary>Логика выполнения - Команда отмены редактирования</summary>
        private void OnCancelEditCommandExecuted(object? p) => (p as Window)!.Close();

        #endregion

        #region SelectReaderCommand - Команда выбрать читателя

        ///<summary>Команда выбрать читателя</summary>
        private ICommand? _selectReaderCommand;

        ///<summary>Команда выбрать читателя</summary>
        public ICommand SelectReaderCommand => _selectReaderCommand
            ??= new LambdaCommand(OnSelectReaderCommandExecuted);

        ///<summary>Логика выполнения - Команда выбрать читателя</summary>
        private void OnSelectReaderCommandExecuted(object? p)
        {
            var viewModel = new SelectReaderForEditorBookViewModel(this);
            var window = new SelectReaderForEditorBookWindow { DataContext = viewModel };
            window.Closed += (_, _) =>
            {
                viewModel = null;
                window = null;
            };
            window.ShowDialog();
        }

        #endregion

        #endregion

        public EditorBookViewModel(BooksViewModel booksVM, ReadersViewModel readersVM)
        {
            _booksVM = booksVM;
            _readersVM = readersVM;
            _bookOnEdit = _booksVM.SelectedBook!;
            _indexBook = _booksVM.Books.IndexOf(_bookOnEdit);
            SelectedCodeAuthor = _bookOnEdit.CodeAuthor;
            SelectedBBK = _bookOnEdit.BBK;
            SelectedAuthor = _bookOnEdit.Author;
            SelectedName = _bookOnEdit.Name;
            SelectedPublish = _bookOnEdit.Publish;
            SelectedYearPublish = _bookOnEdit.YearPublish;
            SelectedPage = _bookOnEdit.Pages;
            SelectedISBN = _bookOnEdit.ISBN;
            Existence = _bookOnEdit.Existence == "Да";
        }
    }
}
