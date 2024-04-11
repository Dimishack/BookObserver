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
        private readonly int? _indexReaderFromBook;

        #region Existence : bool - В наличии

        ///<summary>В наличии</summary>
        private bool _existence;

        ///<summary>В наличии</summary>
        public bool Existence
        {
            get => _existence;
            set
            {
                if (!Set(ref _existence, value)) return;
                if (!value)
                    DateGet = DateTime.Now;
                else
                {
                    IndexReader = null;
                    FullNameReader = null;
                    DateGet = null;
                    DateSet = null;
                }
            }
        }

        #endregion

        #region BBK : string - ББК

        ///<summary>ББК</summary>
        private string _bbk = string.Empty;

        ///<summary>ББК</summary>
        public string BBK { get => _bbk; set => Set(ref _bbk, value); }

        #endregion

        #region Author : string - Автор

        ///<summary>Автор</summary>
        private string _author = string.Empty;

        ///<summary>Автор</summary>
        public string Author { get => _author; set => Set(ref _author, value); }

        #endregion

        #region Name : string - Название

        ///<summary>Название</summary>
        private string _name = string.Empty;

        ///<summary>Название</summary>
        public string Name { get => _name; set => Set(ref _name, value); }

        #endregion

        #region Publish : string - Издательство

        ///<summary>Издательство</summary>
        private string _publish = string.Empty;

        ///<summary>Издательство</summary>
        public string Publish { get => _publish; set => Set(ref _publish, value); }

        #endregion

        #region YearPublish : string - Год издания

        ///<summary>Год издания</summary>
        private string _yearPublish = string.Empty;

        ///<summary>Год издания</summary>
        public string YearPublish { get => _yearPublish; set => Set(ref _yearPublish, value); }

        #endregion

        #region Pages : string - Количество страниц

        ///<summary>Количество страниц</summary>
        private string _pages = string.Empty;

        ///<summary>Количество страниц</summary>
        public string Pages { get => _pages; set => Set(ref _pages, value); }

        #endregion

        #region ISBN : string - ISBN

        ///<summary>ISBN</summary>
        private string _isbn = string.Empty;

        ///<summary>ISBN</summary>
        public string ISBN { get => _isbn; set => Set(ref _isbn, value); }

        #endregion

        #region IndexReader : int? - Id выбранного читателя

        ///<summary>Id выбранного читателя</summary>
        private int? _indexReader;

        ///<summary>Id выбранного читателя</summary>
        public int? IndexReader { get => _indexReader; set => Set(ref _indexReader, value); }

        #endregion

        #region FullNameReader : string? - ФИО читателя

        ///<summary>ФИО читателя</summary>
        private string? _fullNameReader;

        ///<summary>ФИО читателя</summary>
        public string? FullNameReader { get => _fullNameReader; set => Set(ref _fullNameReader, value); }

        #endregion

        #region DateGet : DateTime? - Дата получения

        ///<summary>Дата получения</summary>
        private DateTime? _dateGet;

        ///<summary>Дата получения</summary>
        public DateTime? DateGet
        {
            get => _dateGet;
            set
            {
                if (!Set(ref _dateGet, value)) return;

                if (value is not null)
                    DateSet = value.Value.AddMonths(1);
            }
        }

        #endregion

        #region DateSet : DateTime? - Выбранная дата возврата

        ///<summary>Выбранная дата возврата</summary>
        private DateTime? _dateSet;

        ///<summary>Выбранная дата возврата</summary>
        public DateTime? DateSet { get => _dateSet; set => Set(ref _dateSet, value); }

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
            _bbk != _bookOnEdit.BBK
            || _author != _bookOnEdit.Author
            || _name != _bookOnEdit.Name
            || _publish != _bookOnEdit.Publish
            || _yearPublish != _bookOnEdit.YearPublish
            || _pages != _bookOnEdit.Pages
            || _isbn != _bookOnEdit.ISBN
            || _fullNameReader != _bookOnEdit.FullNameReader
            || _dateGet != _bookOnEdit.DateGet
            || _dateSet != _bookOnEdit.DateSet
            || _indexReader != _indexReaderFromBook
            ;

        ///<summary>Логика выполнения - Команда возвращения в первоначальный вид</summary>
        private void OnResetCommandExecuted(object? p)
        {
            BBK = _bookOnEdit.BBK;
            Author = _bookOnEdit.Author;
            Name = _bookOnEdit.Name;
            Publish = _bookOnEdit.Publish;
            YearPublish = _bookOnEdit.YearPublish;
            Pages = _bookOnEdit.Pages;
            ISBN = _bookOnEdit.ISBN;
            Existence = _bookOnEdit.Existence == "Да";
            FullNameReader = _bookOnEdit.FullNameReader;
            DateGet = _bookOnEdit.DateGet;
            DateSet = _bookOnEdit.DateSet;
            IndexReader = _indexReaderFromBook;
        }

        #endregion

        #region EditBookCommand - Редактировать книгу (нажатие на ОК)

        ///<summary>Редактировать книгу (нажатие на ОК)</summary>
        private ICommand? _editBookCommand;

        ///<summary>Редактировать книгу (нажатие на ОК)</summary>
        public ICommand EditBookCommand => _editBookCommand
            ??= new LambdaCommand(OnEditBookCommandExecuted, CanEditBookCommandExecute);

        ///<summary>Проверка возможности выполнения - Редактировать книгу (нажатие на ОК)</summary>
        private bool CanEditBookCommandExecute(object? p)
        {
            if (p is not Window) return false;

            if ( _bbk == _bookOnEdit.BBK
            && _author == _bookOnEdit.Author
            && _name == _bookOnEdit.Name
            && _publish == _bookOnEdit.Publish
            && _yearPublish == _bookOnEdit.YearPublish
            && _pages == _bookOnEdit.Pages
            && _isbn == _bookOnEdit.ISBN
            && _fullNameReader == _bookOnEdit.FullNameReader
            && _dateGet == _bookOnEdit.DateGet
            && _dateSet == _bookOnEdit.DateSet
            && _existence == (_bookOnEdit.Existence == "Да")) return false;

            if (!_existence && (string.IsNullOrWhiteSpace(_fullNameReader)
                || _dateGet is null
                || _dateSet is null)) return false;

            return true;
        }


        ///<summary>Логика выполнения - Редактировать книгу (нажатие на ОК)</summary>
        private void OnEditBookCommandExecuted(object? p)
        {
            var window = (p as Window)!;
            window.DialogResult = true;
            _booksVM.Books[_indexBook] = new Book
            {
                Id = _bookOnEdit.Id,
                IndexReader = _indexReader,
                BBK = _bbk,
                Author = _author,
                Name = _name,
                Publish = _publish,
                YearPublish = _yearPublish,
                Pages = _pages,
                ISBN = _isbn,
                Existence = _existence ? "Да" : "Нет",
                FullNameReader = _fullNameReader,
                DateGet = _dateGet,
                DateSet = _dateSet
            };
            if (_indexReaderFromBook is not null && _indexReaderFromBook != _indexReader)
            {
                _readersVM.Readers[(int)_indexReaderFromBook].IndexesBooks.Remove(_indexBook);
                if(_readersVM.Readers[(int)_indexReaderFromBook].IndexesBooks.Count <= 0)
                    _readersVM.Readers[(int)_indexReaderFromBook].BooksWithHim = "Нет";

            }
            if (_indexReader is not null && !_readersVM.Readers[(int)_indexReader].IndexesBooks.Contains(_indexBook))
            {
                _readersVM.Readers[(int)_indexReader].IndexesBooks.Add(_indexBook);
                _readersVM.Readers[(int)_indexReader].BooksWithHim = "Да";
            }
                _readersVM._readersView.View.Refresh();
            window.Close();
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
            var viewModel = new SelectReaderForEditorBookViewModel(this, _readersVM);
            var window = new SelectReaderForEditorBookWindow { DataContext = viewModel };
            window.Closed += (_, _) =>
            {
                viewModel = null;
                window = null;
                ClearGarbage();
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
            BBK = _bookOnEdit.BBK;
            Author = _bookOnEdit.Author;
            Name = _bookOnEdit.Name;
            Publish = _bookOnEdit.Publish;
            YearPublish = _bookOnEdit.YearPublish;
            Pages = _bookOnEdit.Pages;
            ISBN = _bookOnEdit.ISBN;
            FullNameReader = _bookOnEdit.FullNameReader;
            DateGet = _bookOnEdit.DateGet;
            DateSet = _bookOnEdit.DateSet;
            Existence = _bookOnEdit.Existence == "Да";
            _indexReaderFromBook = IndexReader = _bookOnEdit.IndexReader;
        }
    }
}
