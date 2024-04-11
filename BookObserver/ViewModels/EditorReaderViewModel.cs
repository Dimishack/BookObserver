using BookObserver.Infrastructure.Commands;
using BookObserver.Models.Books;
using BookObserver.Models.Readers;
using BookObserver.ViewModels.Base;
using BookObserver.Views.Windows;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    internal class EditorReaderViewModel : ViewModel
    {
        private readonly BooksViewModel _booksVM;
        private readonly ReadersViewModel _readersVM;

        private readonly Reader _readerOnEdit;
        private readonly int _indexReader;

        private ObservableCollection<int> _indexesBooksOnDelete { get; } = [];

        ///<summary>Выбранные книги</summary>
        public ObservableCollection<Book> SelectedBooks { get; } = [];

        #region LastName : string - Выбранная фамилия

        ///<summary>Выбранная фамилия</summary>
        private string _lastName = string.Empty;

        ///<summary>Выбранная фамилия</summary>
        public string LastName { get => _lastName; set => Set(ref _lastName, value); }

        #endregion

        #region FirstName : string - Выбранное имя

        ///<summary>Выбранное имя</summary>
        private string _firstName = string.Empty;

        ///<summary>Выбранное имя</summary>
        public string FirstName { get => _firstName; set => Set(ref _firstName, value); }

        #endregion

        #region Patronymic : string - Выбранное отчество

        ///<summary>Выбранное отчество</summary>
        private string _patronymic = string.Empty;

        ///<summary>Выбранное отчество</summary>
        public string Patronymic { get => _patronymic; set => Set(ref _patronymic, value); }

        #endregion

        #region Address : string - Выбранный адрес

        ///<summary>Выбранный адрес</summary>
        private string _address = string.Empty;

        ///<summary>Выбранный адрес</summary>
        public string Address { get => _address; set => Set(ref _address, value); }

        #endregion

        #region PhoneNumber : string - Выбранный номер телефона

        ///<summary>Выбранный номер телефона</summary>
        private string _phoneNumber = string.Empty;

        ///<summary>Выбранный номер телефона</summary>
        public string PhoneNumber { get => _phoneNumber; set => Set(ref _phoneNumber, value); }

        #endregion

        #region HomePhoneNumber : string - Выбранный домашний номер телефона

        ///<summary>Выбранный домашний номер телефона</summary>
        private string _homePhoneNumber = string.Empty;

        ///<summary>Выбранный домашний номер телефона</summary>
        public string HomePhoneNumber { get => _homePhoneNumber; set => Set(ref _homePhoneNumber, value); }

        #endregion

        #region BooksWithHim : bool - Книги с собой?

        ///<summary>Книги с собой?</summary>
        private bool _booksWithHim;

        ///<summary>Книги с собой?</summary>
        public bool BooksWithHim { get => _booksWithHim; set => Set(ref _booksWithHim, value); }

        #endregion

        #region IndexesBook : ObservableCollection<int> - Коллекция индексов книг

        ///<summary>Коллекция индексов книг</summary>
        private ObservableCollection<int> _indexesBook = [];

        ///<summary>Коллекция индексов книг</summary>
        public ObservableCollection<int> IndexesBooks { get => _indexesBook; set => Set(ref _indexesBook, value); }

        #endregion

        #region SelectedBook : Book? - Выбранная книга

        ///<summary>Выбранная книга</summary>
        private Book? _selectedBook;

        ///<summary>Выбранная книга</summary>
        public Book? SelectedBook
        {
            get => _selectedBook;
            set
            {
                if (!Set(ref _selectedBook, value) || value is null) return;

                if (value.DateGet == null)
                    DateGetSelectedBook = DateTime.Now;
                else
                {
                    DateGetSelectedBook = value.DateGet;
                    DateSetSelectedBook = value.DateSet;
                }
            }
        }

        #endregion

        #region DateGetSelectedBook : DateTime? - Дата получения выбранной книги

        ///<summary>Дата получения выбранной книги</summary>
        private DateTime? _dateGetSelectedBook;

        ///<summary>Дата получения выбранной книги</summary>
        public DateTime? DateGetSelectedBook
        {
            get => _dateGetSelectedBook;
            set
            {
                if (!Set(ref _dateGetSelectedBook, value)) return;

                SelectedBook!.DateGet = value;
                if (value is not null)
                    DateSetSelectedBook = value.Value.AddMonths(1);
            }
        }

        #endregion

        #region DateSetSelectedBook : DateTime? - Дата возврата выбранной книги

        ///<summary>Дата возврата выбранной книги</summary>
        private DateTime? _dateSetSelectedBook;

        ///<summary>Дата возврата выбранной книги</summary>
        public DateTime? DateSetSelectedBook
        {
            get => _dateSetSelectedBook;
            set
            {
                if (!Set(ref _dateSetSelectedBook, value)) return;

                SelectedBook!.DateSet = value;
            }
        }

        #endregion

        #region Commands

        #region ResetCommand - Команда возвращения в первоначальный вид

        ///<summary>Команда возвращения в первоначальный вид</summary>
        private ICommand? _resetCommand;

        ///<summary>Команда возвращения в первоначальный вид</summary>
        public ICommand ResetCommand => _resetCommand
            ??= new LambdaCommand(OnResetCommandExecuted, CanResetCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда возвращения в первоначальный вид</summary>
        private bool CanResetCommandExecute(object? p) =>
            _lastName != _readerOnEdit.LastName
            || _firstName != _readerOnEdit.FirstName
            || _patronymic != _readerOnEdit.Patronymic
            || _phoneNumber != _readerOnEdit.PhoneNumber
            || _homePhoneNumber != _readerOnEdit.HomePhoneNumber
            || _address != _readerOnEdit.Address
            || _booksWithHim != (_readerOnEdit.BooksWithHim == "Да")
            || !_indexesBook.SequenceEqual(_readerOnEdit.IndexesBooks)
            ;

        ///<summary>Логика выполнения - Команда возвращения в первоначальный вид</summary>
        private void OnResetCommandExecuted(object? p)
        {
            LastName = _readerOnEdit.LastName;
            FirstName = _readerOnEdit.FirstName;
            Patronymic = _readerOnEdit.Patronymic;
            PhoneNumber = _readerOnEdit.PhoneNumber;
            HomePhoneNumber = _readerOnEdit.HomePhoneNumber;
            Address = _readerOnEdit.Address;
            BooksWithHim = _readerOnEdit.BooksWithHim == "Да";
            IndexesBooks = new(_readerOnEdit.IndexesBooks);
            SelectedBooks.Clear();
            foreach (var indexBook in IndexesBooks)
                SelectedBooks.Add(_booksVM.Books[indexBook]);
        }

        #endregion

        #region CancelEditCommand - Команда отмена редактирования

        ///<summary>Команда отмена редактирования</summary>
        private ICommand? _cancelEditCommand;

        ///<summary>Команда отмена редактирования</summary>
        public ICommand CancelEditCommand => _cancelEditCommand
            ??= new LambdaCommand(OnCancelEditCommandExecuted, CanCancelEditCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда отмена редактирования</summary>
        private bool CanCancelEditCommandExecute(object? p) => p is Window;

        ///<summary>Логика выполнения - Команда отмена редактирования</summary>
        private void OnCancelEditCommandExecuted(object? p) => (p as Window)!.Close();

        #endregion

        #region EditCommand - Команда редактировать читателя

        ///<summary>Команда редактировать читателя</summary>
        private ICommand? _editCommand;

        ///<summary>Команда редактировать читателя</summary>
        public ICommand EditCommand => _editCommand
            ??= new LambdaCommand(OnEditCommandExecuted, CanEditCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда редактировать читателя</summary>
        private bool CanEditCommandExecute(object? p) =>
            p is Window
            && (
            !_booksWithHim
            || _indexesBook.Count > 0
            )
            ;

        ///<summary>Логика выполнения - Команда редактировать читателя</summary>
        private void OnEditCommandExecuted(object? p)
        {
            var window = (p as Window)!;
            var reader = new Reader
            {
                Id = _indexReader,
                LastName = _lastName,
                FirstName = _firstName,
                Patronymic = _patronymic,
                Address = _address,
                BooksWithHim = _booksWithHim ? "Да" : "Нет",
                PhoneNumber = _phoneNumber,
                HomePhoneNumber = _homePhoneNumber,
                IndexesBooks = _indexesBook,
            };
            _readersVM.Readers[_indexReader] = reader;
            if (_indexesBook.Count > 0 || _indexesBooksOnDelete.Count > 0)
            {
                for (int i = 0; i < _indexesBook.Count; i++)
                {
                    _booksVM.Books[_indexesBook[i]].Existence = "Нет";
                    _booksVM.Books[_indexesBook[i]].FullNameReader = reader.FullName;
                    _booksVM.Books[_indexesBook[i]].DateGet = SelectedBooks[i].DateGet;
                    _booksVM.Books[_indexesBook[i]].DateSet = SelectedBooks[i].DateSet;
                }
                foreach (var index in _indexesBooksOnDelete)
                {
                    _booksVM.Books[index].Existence = "Да";
                    _booksVM.Books[index].FullNameReader = null;
                    _booksVM.Books[index].DateGet = null;
                    _booksVM.Books[index].DateSet = null;
                }
                _booksVM._booksView.View.Refresh();
            }
            window.DialogResult = true;
            window.Close();
        }

        #endregion

        #region SelectBooksCommand - Команда выбрать книги

        ///<summary>Команда выбрать книги</summary>
        private ICommand? _selectBooksCommand;

        ///<summary>Команда выбрать книги</summary>
        public ICommand SelectBooksCommand => _selectBooksCommand
            ??= new LambdaCommand(OnSelectBooksCommandExecuted);

        ///<summary>Логика выполнения - Команда выбрать книги</summary>
        private void OnSelectBooksCommandExecuted(object? p)
        {
            var viewModel = new SelectBooksForEditorReaderViewModel(this, _booksVM);
            var window = new SelectBooksForEditorReaderWindow
            {
                DataContext = viewModel,
                Owner = App.ActiveWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            window.Closed += (_, _) =>
            {
                viewModel = null;
                window = null;
                ClearGarbage();
            };
            if (window.ShowDialog() == true)
            {
                SelectedBooks.Add(_booksVM.Books[IndexesBooks[^1]]);
                SelectedBook = SelectedBooks[^1];
            }
        }

        #endregion

        #region DeleteBookCommand - Команда - удаление книги

        ///<summary>Команда - удаление книги</summary>
        private ICommand? _deleteBookCommand;

        ///<summary>Команда - удаление книги</summary>
        public ICommand DeleteBookCommand => _deleteBookCommand
            ??= new LambdaCommand(OnDeleteBookCommandExecuted, CanDeleteBookCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда - удаление книги</summary>
        private bool CanDeleteBookCommandExecute(object? p) => p is Book;

        ///<summary>Логика выполнения - Команда - удаление книги</summary>
        private void OnDeleteBookCommandExecuted(object? p)
        {
            var book = (p as Book)!;
            _indexesBooksOnDelete.Add(IndexesBooks[SelectedBooks.IndexOf(book)]);
            IndexesBooks.RemoveAt(SelectedBooks.IndexOf(book));
            SelectedBooks.Remove(book);
            if (SelectedBooks.Count <= 0)
                BooksWithHim = false;
        }

        #endregion

        #endregion

        public EditorReaderViewModel(BooksViewModel booksVM, ReadersViewModel readersVM)
        {
            _booksVM = booksVM;
            _readersVM = readersVM;
            _readerOnEdit = _readersVM.SelectedReader!;
            _indexReader = _readerOnEdit.Id;
            LastName = _readerOnEdit.LastName;
            FirstName = _readerOnEdit.FirstName;
            Patronymic = _readerOnEdit.Patronymic;
            PhoneNumber = _readerOnEdit.PhoneNumber;
            HomePhoneNumber = _readerOnEdit.HomePhoneNumber;
            Address = _readerOnEdit.Address;
            BooksWithHim = _readerOnEdit.BooksWithHim == "Да";
            IndexesBooks = new(_readerOnEdit.IndexesBooks);
            foreach (var indexBook in IndexesBooks)
                SelectedBooks.Add(_booksVM.Books[indexBook]);
        }
    }
}
