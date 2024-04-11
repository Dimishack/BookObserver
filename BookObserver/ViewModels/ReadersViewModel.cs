using BookObserver.Infrastructure.Commands;
using BookObserver.Infrastructure.Commands.Base;
using BookObserver.Models;
using BookObserver.Models.Books;
using BookObserver.Models.Readers;
using BookObserver.Services.Interfaces;
using BookObserver.ViewModels.Base;
using BookObserver.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    internal class ReadersViewModel : ViewModel
    {
        private readonly BooksViewModel _booksVM;

        private IUserDialog _userDialog;
        private CreatorReaderWindow? _creatorReaderWindow;
        private EditorReaderWindow? _editorReaderWindow;
        public Dictionary<string, SortDescription> Sorting { get; } = new()
        {
            {"Сначала старые записи", new SortDescription("Id", ListSortDirection.Ascending)},
            {"Сначала новые записи", new SortDescription("Id", ListSortDirection.Descending)},
            {"Фамилии (по возрастанию)", new SortDescription("LastName", ListSortDirection.Ascending)},
            {"Фамилии (по убыванию)", new SortDescription("LastName", ListSortDirection.Descending)},
            {"Имена (по возрастанию)", new SortDescription("FirstName", ListSortDirection.Ascending)},
            {"Имена (по убыванию)", new SortDescription("FirstName", ListSortDirection.Descending)},
            {"Отчества (по возрастанию)", new SortDescription("Patronymic", ListSortDirection.Ascending)},
            {"Отчества (по убыванию)", new SortDescription("Patronymic", ListSortDirection.Descending)},
        };

        #region SelectedSorting : string - Выбранная сортировка списка читателей

        ///<summary>Выбранная сортировка списка читателей</summary>
        private string _selectedSorting = "Сначала старые записи";

        ///<summary>Выбранная сортировка списка читателей</summary>
        public string SelectedSorting
        {
            get => _selectedSorting;
            set
            {
                if (!Set(ref _selectedSorting, value)) return;

                _readersView.View.SortDescriptions.Clear();
                ClearGarbage();
                _readersView.View.SortDescriptions.Add(Sorting[value]);
            }
        }

        #endregion

        /// <summary>Список книг</summary>
        public ObservableCollection<Reader> Readers { get; }

        #region FiltredReaders : ObservableCollection<Reader>? - Отфильтрованный список читателей

        ///<summary>Отфильтрованный список читателей</summary>
        private ObservableCollection<Reader>? _filtredReaders;

        ///<summary>Отфильтрованный список читателей</summary>
        private ObservableCollection<Reader>? FiltredReaders { get => _filtredReaders; set => Set(ref _filtredReaders, value); }

        #endregion

        #region ReadersView : ICollectionView - Вывод списка читателей

        public readonly CollectionViewSource _readersView = new();
        public ICollectionView ReadersView => _readersView.View;
        #endregion

        #region SelectedReader : Reader? - Выбранный читатель

        ///<summary>Выбранный читатель</summary>
        private Reader? _selectedReader;

        ///<summary>Выбранный читатель</summary>
        public Reader? SelectedReader { get => _selectedReader; set => Set(ref _selectedReader, value); }

        #endregion

        private readonly CollectionWithFilter _lastNamesView = new();
        public ObservableCollection<string>? LastNamesView => _lastNamesView.CollectionView;

        #region SelectedLastName : string? - Выбранная фамилия

        ///<summary>Выбранная фамилия</summary>
        private string _selectedLastName = string.Empty;

        ///<summary>Выбранная фамилия</summary>
        public string SelectedLastName
        {
            get => _selectedLastName;
            set
            {
                if (!Set(ref _selectedLastName, value)) return;

                _lastNamesView.RefreshFilter(value);
                OnPropertyChanged(nameof(LastNamesView));
                ((Command)SearchCommand).Executable = true;
            }
        }

        #endregion

        private readonly CollectionWithFilter _firstNamesView = new();
        public ObservableCollection<string>? FirstNamesView => _firstNamesView.CollectionView;

        #region SelectedFirstName : string? - Выбранное имя

        ///<summary>Выбранное имя</summary>
        private string _selectedFirstName = string.Empty;

        ///<summary>Выбранное имя</summary>
        public string SelectedFirstName
        {
            get => _selectedFirstName;
            set
            {
                if (!Set(ref _selectedFirstName, value)) return;

                _firstNamesView.RefreshFilter(value);
                OnPropertyChanged(nameof(FirstNamesView));
                ((Command)SearchCommand).Executable = true;
            }
        }

        #endregion

        private readonly CollectionWithFilter _patronymicsView = new();
        public ObservableCollection<string>? PatronymicsView => _patronymicsView.CollectionView;

        #region SelectedPatronymic : string - Выбранное отчество

        ///<summary>Выбранное отчество</summary>
        private string _selectedPatronymic = string.Empty;

        ///<summary>Выбранное отчество</summary>
        public string SelectedPatronymic
        {
            get => _selectedPatronymic;
            set
            {
                if (!Set(ref _selectedPatronymic, value)) return;

                _patronymicsView.RefreshFilter(value);
                OnPropertyChanged(nameof(PatronymicsView));
                ((Command)SearchCommand).Executable = true;
            }
        }

        #endregion

        #region Commands

        #region AddReaderCommand - Команда добавления читателя

        ///<summary>Команда добавления читателя</summary>
        private ICommand? _addReaderCommand;

        ///<summary>Команда добавления читателя</summary>
        public ICommand AddReaderCommand => _addReaderCommand
            ??= new LambdaCommand(OnAddReaderCommandExecuted);

        ///<summary>Логика выполнения - Команда добавления читателя</summary>
        private void OnAddReaderCommandExecuted(object? p)
        {
            if (_creatorReaderWindow is { } window)
            {
                window.Show();
                return;
            }
            window = App.Services.GetRequiredService<CreatorReaderWindow>();
            window.Closed += (_, _) =>
            {
                _creatorReaderWindow = null;
                ClearGarbage();
            };

            _creatorReaderWindow = window;
            _creatorReaderWindow.Show();
            ((Command)SaveReadersCommand).Executable = true;
        }

        #endregion

        #region EditReaderCommand - Команда редактирования читателя

        ///<summary>Команда редактирования читателя</summary>
        private ICommand? _editReaderCommand;

        ///<summary>Команда редактирования читателя</summary>
        public ICommand EditReaderCommand => _editReaderCommand
            ??= new LambdaCommand(OnEditorReaderCommandExecuted, CanEditorReaderCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда редактирования читателя</summary>
        private bool CanEditorReaderCommandExecute(object? p) => _selectedReader is not null;

        ///<summary>Логика выполнения - Команда редактирования читателя</summary>
        private void OnEditorReaderCommandExecuted(object? p)
        {
            if (_editorReaderWindow is { } window)
            {
                _editorReaderWindow.ShowDialog();
                return;
            }
            window = App.Services.GetRequiredService<EditorReaderWindow>();
            window.Closed += (_, _) =>
            {
                _editorReaderWindow = null;
                ClearGarbage();
            };
            _editorReaderWindow = window;
            if (_editorReaderWindow.ShowDialog() == false) return;

            ((Command)SaveReadersCommand).Executable = true;
            _userDialog.ShowInformation("Запись успешно отформатирована.", "Редактировать запись");
        }

        #endregion

        #region DeleteReaderCommand - Команда удаления читателя

        ///<summary>Команда удаления читателя</summary>
        private ICommand? _deleteReaderCommand;

        ///<summary>Команда удаления читателя</summary>
        public ICommand DeleteReaderCommand => _deleteReaderCommand
            ??= new LambdaCommand(OnDeleteReaderCommandExecuted, CanDeleteReaderCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда удаления читателя</summary>
        private bool CanDeleteReaderCommandExecute(object? p) => p is Reader;

        ///<summary>Логика выполнения - Команда удаления читателя</summary>
        private void OnDeleteReaderCommandExecuted(object? p)
        {
            Reader reader = (p as Reader)!;
            var index = Readers.IndexOf(reader);
            if (Readers.Remove(reader))
                for (int i = index; i < Readers.Count; i++)
                    Readers[i].Id--;
            FiltredReaders?.Remove(reader);
            ((Command)SaveReadersCommand).Executable = true;
        }

        #endregion

        #region SaveReadersCommand - Команда сохранение читателей

        ///<summary>Команда сохранение читателей</summary>
        private ICommand? _saveReadersCommand;

        ///<summary>Команда сохранение читателей</summary>
        public ICommand SaveReadersCommand => _saveReadersCommand
            ??= new LambdaCommand(OnSaveReadersCommandExecuted, CanSaveReadersCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда сохранение читателей</summary>
        private bool CanSaveReadersCommandExecute(object? p) => p is not null
            && p is IList<Reader>
            ;

        ///<summary>Логика выполнения - Команда сохранение читателей</summary>
        private async void OnSaveReadersCommandExecuted(object? p)
        {
            using (var writer = new StreamWriter($@"{Environment.CurrentDirectory}/Data/Readers.json"))
                await writer.WriteAsync(JsonConvert.SerializeObject(p, Formatting.Indented));
            ((Command)SaveReadersCommand).Executable = false;
            _userDialog?.ShowInformation("Файл успешно сохранен", "BookObserver");
        }

        #endregion

        #region ClearFieldsForSearchCommand - Команда очистка полей для поиска

        ///<summary>Команда очистка полей для поиска</summary>
        private ICommand? _clearFieldsForSearchCommand;

        ///<summary>Команда очистка полей для поиска</summary>
        public ICommand ClearFieldsForSearchCommand => _clearFieldsForSearchCommand
            ??= new LambdaCommand(OnClearFieldsForSearchCommandExecuted, CanClearFieldsForSearchCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда очистка полей для поиска</summary>
        private bool CanClearFieldsForSearchCommandExecute(object? p) =>
            !string.IsNullOrWhiteSpace(_selectedLastName)
            || !string.IsNullOrWhiteSpace(_selectedFirstName)
            || !string.IsNullOrWhiteSpace(_selectedPatronymic)
            ;

        ///<summary>Логика выполнения - Команда очистка полей для поиска</summary>
        private void OnClearFieldsForSearchCommandExecuted(object? p)
        {
            SelectedLastName = string.Empty;
            SelectedFirstName = string.Empty;
            SelectedPatronymic = string.Empty;
        }

        #endregion

        #region SearchCommand - Команда поиска

        ///<summary>Команда поиска</summary>
        private ICommand? _searchCommand;

        ///<summary>Команда поиска</summary>
        public ICommand SearchCommand => _searchCommand
            ??= new LambdaCommand(OnSearchCommandExecuted, CanSearchCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда поиска</summary>
        private bool CanSearchCommandExecute(object? p) =>
            (p is not null && p is IList<Reader>)
            &&
            (!string.IsNullOrWhiteSpace(_selectedLastName)
            || !string.IsNullOrWhiteSpace(_selectedFirstName)
            || !string.IsNullOrWhiteSpace(_selectedPatronymic)
            );

        ///<summary>Логика выполнения - Команда поиска</summary>
        private void OnSearchCommandExecuted(object? p)
        {
            IList<Reader> result = new ObservableCollection<Reader>((p as IList<Reader>)!);
            if (!string.IsNullOrWhiteSpace(_selectedLastName))
                result = result.Where(r => r.LastName.Contains(_selectedLastName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_selectedFirstName))
                result = result.Where(r => r.FirstName.Contains(_selectedFirstName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_selectedPatronymic))
                result = result.Where(r => r.Patronymic.Contains(_selectedPatronymic, StringComparison.OrdinalIgnoreCase)).ToList();

            FiltredReaders = null;
            _readersView.View.SortDescriptions.Clear();
            ClearGarbage();
            _readersView.Source = FiltredReaders = new(result);
            _readersView.View.SortDescriptions.Add(Sorting[_selectedSorting]);
            OnPropertyChanged(nameof(ReadersView));
            ((Command)SearchCommand).Executable = false;
            ((Command)ResetToZeroSearchCommand).Executable = true;
        }

        #endregion

        #region ResetToZeroSearchCommand - Команда обнуления поиска

        ///<summary>Команда обнуления поиска</summary>
        private ICommand? _resetToZeroSearchCommand;

        ///<summary>Команда обнуления поиска</summary>
        public ICommand ResetToZeroSearchCommand => _resetToZeroSearchCommand
            ??= new LambdaCommand(OnResetToZeroSearchCommandExecuted, CanResetToZeroSearchCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда обнуления поиска</summary>
        private bool CanResetToZeroSearchCommandExecute(object? p) =>
            p is not null && p is IList<Reader>;

        ///<summary>Логика выполнения - Команда обнуления поиска</summary>
        private void OnResetToZeroSearchCommandExecuted(object? p)
        {
            _readersView.Source = FiltredReaders = Readers;
            OnPropertyChanged(nameof(ReadersView));
            ((Command)ResetToZeroSearchCommand).Executable = false;
            ((Command)SearchCommand).Executable = true;
        }

        #endregion

        #region ShowBooksOfReaderCommand - Команда показать список книг у читателя

        ///<summary>Команда показать список книг у читателя</summary>
        private ICommand? _showBooksOfReaderCommand;

        ///<summary>Команда показать список книг у читателя</summary>
        public ICommand ShowBooksOfReaderCommand => _showBooksOfReaderCommand
            ??= new LambdaCommand(OnShowBooksOfReaderCommandExecuted, CanShowBooksOfReaderCommandExecute);

        private bool CanShowBooksOfReaderCommandExecute(object? p) => p is Reader;

        ///<summary>Логика выполнения - Команда показать список книг у читателя</summary>
        private void OnShowBooksOfReaderCommandExecuted(object? p)
        {
            var books = new ObservableCollection<Book>();
            foreach (var indexBook in (p as Reader)!.IndexesBooks)
                books.Add(_booksVM.Books[indexBook]);
            var window = new BooksFromReaderWindow
            {
                Books = books
            };
            window.Closed += (_, _) =>
            {
                window = null;
                ClearGarbage();
            };
            window.Show();
        }

        #endregion

        #region GotFocus Commands...

        #region GotFocusComboBoxLastNamesCommand - Команда при получении фокуса (ComboBox фамилии)

        ///<summary>Команда при получении фокуса (ComboBox фамилии)</summary>
        private ICommand? _gotFocusComboBoxLastNamesCommand;

        ///<summary>Команда при получении фокуса (ComboBox фамилии)</summary>
        public ICommand GotFocusComboBoxLastNamesCommand => _gotFocusComboBoxLastNamesCommand
            ??= new LambdaCommand(OnGotFocusComboBoxLastNamesCommandExecuted, CanGotFocusComboBoxLastNamesCommandExecute);

        private bool CanGotFocusComboBoxLastNamesCommandExecute(object? p) =>
            p is not null && p is IList<Reader>;

        ///<summary>Логика выполнения - Команда при получении фокуса (ComboBox фамилии)</summary>
        private void OnGotFocusComboBoxLastNamesCommandExecuted(object? p)
        {
            _lastNamesView.List = (p as IList<Reader>)!
                .Select(r => r.LastName)
                .Distinct()
                .Order()
                .ToList()!;
            OnPropertyChanged(nameof(LastNamesView));
        }

        #endregion

        #region GotFocusComboBoxFirstNamesCommand - Команда при получении фокуса (ComboBox имена)

        ///<summary>Команда при получении фокуса (ComboBox имена)</summary>
        private ICommand? _gotFocusComboBoxFirstNamesCommand;

        ///<summary>Команда при получении фокуса (ComboBox имена)</summary>
        public ICommand GotFocusComboBoxFirstNamesCommand => _gotFocusComboBoxFirstNamesCommand
            ??= new LambdaCommand(OnGotFocusComboBoxFirstNamesCommandExecuted, CanGotFocusComboBoxFirstNamesCommandExecute);

        private bool CanGotFocusComboBoxFirstNamesCommandExecute(object? p) =>
            p is not null && p is IList<Reader>;

        ///<summary>Логика выполнения - Команда при получении фокуса (ComboBox имена)</summary>
        private void OnGotFocusComboBoxFirstNamesCommandExecuted(object? p)
        {
            _firstNamesView.List = (p as IList<Reader>)!
                .Select(r => r.FirstName)
                .Distinct()
                .Order()
                .ToList()!;
            OnPropertyChanged(nameof(FirstNamesView));
        }

        #endregion

        #region GotFocusComboBoxPatronymicsCommand - Команда при получении фокуса (ComboBox отчества)

        ///<summary>Команда при получении фокуса (ComboBox отчества)</summary>
        private ICommand? _gotFocusComboBoxPatronymicsCommand;

        ///<summary>Команда при получении фокуса (ComboBox отчества)</summary>
        public ICommand GotFocusComboBoxPatronymicsCommand => _gotFocusComboBoxPatronymicsCommand
            ??= new LambdaCommand(OnGotFocusComboBoxPatronymicsCommandExecuted, CanGotFocusComboBoxPatronymicsCommandExecute);

        private bool CanGotFocusComboBoxPatronymicsCommandExecute(object? p) =>
            p is not null && p is IList<Reader>;

        ///<summary>Логика выполнения - Команда при получении фокуса (ComboBox отчества)</summary>
        private void OnGotFocusComboBoxPatronymicsCommandExecuted(object? p)
        {
            _patronymicsView.List = (p as IList<Reader>)!
                .Select(r => r.Patronymic)
                .Distinct()
                .Order()
                .ToList()!;
            OnPropertyChanged(nameof(PatronymicsView));
        }

        #endregion

        #endregion

        #endregion

        public ReadersViewModel(IUserDialog userDialog, BooksViewModel booksVM)
        {
            _booksVM = booksVM;
            _userDialog = userDialog;
            try
            {
                using (var reader = new StreamReader($@"{Environment.CurrentDirectory}/Data/Readers.json"))
                {
                    Readers = JsonConvert.DeserializeObject<ObservableCollection<Reader>>(reader.ReadToEnd());
                }
                Readers ??= [];
            }
            catch (Exception)
            {
                Readers = [];
            }
            finally
            {
                ((Command)ResetToZeroSearchCommand).Executable = false;
                ((Command)SaveReadersCommand).Executable = false;
                _readersView.Source = FiltredReaders = Readers;
            }
        }
    }
}
