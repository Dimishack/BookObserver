using BookObserver.Infrastructure.Commands;
using BookObserver.Models.Books;
using BookObserver.Models.Readers;
using BookObserver.ViewModels.Base;
using BookObserver.Views.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        #region SelectedLastName : string - Выбранная фамилия

        ///<summary>Выбранная фамилия</summary>
        private string _selectedLastName = string.Empty;

        ///<summary>Выбранная фамилия</summary>
        public string SelectedLastName { get => _selectedLastName; set => Set(ref _selectedLastName, value); }

        #endregion

        #region SelectedFirstName : string - Выбранное имя

        ///<summary>Выбранное имя</summary>
        private string _selectedFirstName = string.Empty;

        ///<summary>Выбранное имя</summary>
        public string SelectedFirstName { get => _selectedFirstName; set => Set(ref _selectedFirstName, value); }

        #endregion

        #region SelectedPatronymic : string - Выбранное отчество

        ///<summary>Выбранное отчество</summary>
        private string _selectedPatronymic = string.Empty;

        ///<summary>Выбранное отчество</summary>
        public string SelectedPatronymic { get => _selectedPatronymic; set => Set(ref _selectedPatronymic, value); }

        #endregion

        #region SelectedAddress : string - Выбранный адрес

        ///<summary>Выбранный адрес</summary>
        private string _selectedAddress = string.Empty;

        ///<summary>Выбранный адрес</summary>
        public string SelectedAddress { get => _selectedAddress; set => Set(ref _selectedAddress, value); }

        #endregion

        #region SelectedPhoneNumber : string - Выбранный номер телефона

        ///<summary>Выбранный номер телефона</summary>
        private string _selectedPhoneNumber = string.Empty;

        ///<summary>Выбранный номер телефона</summary>
        public string SelectedPhoneNumber { get => _selectedPhoneNumber; set => Set(ref _selectedPhoneNumber, value); }

        #endregion

        #region SelectedHomePhoneNumber : string - Выбранный домашний номер телефона

        ///<summary>Выбранный домашний номер телефона</summary>
        private string _selectedHomePhoneNumber = string.Empty;

        ///<summary>Выбранный домашний номер телефона</summary>
        public string SelectedHomePhoneNumber { get => _selectedHomePhoneNumber; set => Set(ref _selectedHomePhoneNumber, value); }

        #endregion

        #region BooksWithHim : bool - Книги с собой?

        ///<summary>Книги с собой?</summary>
        private bool _booksWithHim;

        ///<summary>Книги с собой?</summary>
        public bool BooksWithHim { get => _booksWithHim; set => Set(ref _booksWithHim, value); }

        #endregion

        #region IdBooks : ObservableCollection<int> - Коллекция id книг

        ///<summary>Коллекция id книг</summary>
        private ObservableCollection<int> _idBooks = [];

        ///<summary>Коллекция id книг</summary>
        public ObservableCollection<int> IdBooks { get => _idBooks; set => Set(ref _idBooks, value); }

        #endregion

        #region AuthorsAndNamesBooks : ObservableCollection<Tuple<string, string>> - Авторы и названия книг

        ///<summary>Авторы и названия книг</summary>
        private ObservableCollection<Tuple<string, string>> _authorsAndNamesBooks = [];

        ///<summary>Авторы и названия книг</summary>
        public ObservableCollection<Tuple<string, string>> AuthorsAndNamesBooks 
        { get => _authorsAndNamesBooks; set => Set(ref _authorsAndNamesBooks, value); }

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
            _selectedLastName != _readerOnEdit.LastName
            || _selectedFirstName != _readerOnEdit.FirstName
            || _selectedPatronymic != _readerOnEdit.Patronymic
            || _selectedPhoneNumber != _readerOnEdit.PhoneNumber
            || _selectedHomePhoneNumber != _readerOnEdit.HomePhoneNumber
            || _selectedAddress != _readerOnEdit.Address
            || _booksWithHim != (_readerOnEdit.BooksWithHim == "Да")
            ;

        ///<summary>Логика выполнения - Команда возвращения в первоначальный вид</summary>
        private void OnResetCommandExecuted(object? p)
        {
            SelectedLastName = _readerOnEdit.LastName;
            SelectedFirstName = _readerOnEdit.FirstName;
            SelectedPatronymic = _readerOnEdit.Patronymic;
            SelectedPhoneNumber = _readerOnEdit.PhoneNumber;
            SelectedHomePhoneNumber = _readerOnEdit.HomePhoneNumber;
            SelectedAddress = _readerOnEdit.Address;
            BooksWithHim = _readerOnEdit.BooksWithHim == "Да";
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
            || (
            _idBooks.Count > 0
            && _authorsAndNamesBooks.Count > 0
            ))
            ;

        ///<summary>Логика выполнения - Команда редактировать читателя</summary>
        private void OnEditCommandExecuted(object? p)
        {
            _readersVM.Readers[_indexReader] = new Reader
            {
                Id = _indexReader + 1,
                LastName = _selectedLastName,
                FirstName = _selectedFirstName,
                Patronymic = _selectedPatronymic,
                Address = _selectedAddress,
                BooksWithHim = _booksWithHim ? "Да" : "Нет",
                PhoneNumber = _selectedPhoneNumber,
                HomePhoneNumber = _selectedHomePhoneNumber,
                IdBooks = IdBooks,
                AuthorsAndNamesBooks = _authorsAndNamesBooks
            };
            (p as Window)!.Close();
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
            window.ShowDialog();
        }

        #endregion

        #region DeleteBookCommand - Команда - удаление книги

        ///<summary>Команда - удаление книги</summary>
        private ICommand? _deleteBookCommand;

        ///<summary>Команда - удаление книги</summary>
        public ICommand DeleteBookCommand => _deleteBookCommand
            ??= new LambdaCommand(OnDeleteBookCommandExecuted, CanDeleteBookCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда - удаление книги</summary>
        private bool CanDeleteBookCommandExecute(object? p) => p is Tuple<string, string>;

        ///<summary>Логика выполнения - Команда - удаление книги</summary>
        private void OnDeleteBookCommandExecuted(object? p)
        {
            var authorAndName = (p as Tuple<string, string>)!;
            AuthorsAndNamesBooks.Remove(authorAndName);
            IdBooks.Remove(AuthorsAndNamesBooks.IndexOf(authorAndName));
        }

        #endregion

        #endregion

        public EditorReaderViewModel(BooksViewModel booksVM, ReadersViewModel readersVM)
        {
            _booksVM = booksVM;
            _readersVM = readersVM;
            _readerOnEdit = _readersVM.SelectedReader!;
            _indexReader = _readerOnEdit.Id - 1;
            SelectedLastName = _readerOnEdit.LastName;
            SelectedFirstName = _readerOnEdit.FirstName;
            SelectedPatronymic = _readerOnEdit.Patronymic;
            SelectedPhoneNumber = _readerOnEdit.PhoneNumber;
            SelectedHomePhoneNumber = _readerOnEdit.HomePhoneNumber;
            SelectedAddress = _readerOnEdit.Address;
            BooksWithHim = _readerOnEdit.BooksWithHim == "Да";
            AuthorsAndNamesBooks = _readerOnEdit.AuthorsAndNamesBooks;
            IdBooks = _readerOnEdit.IdBooks;
        }
    }
}
