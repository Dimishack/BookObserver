using BookObserver.Infrastructure.Commands;
using BookObserver.Infrastructure.Commands.Base;
using BookObserver.Models;
using BookObserver.Models.Books;
using BookObserver.Services.Interfaces;
using BookObserver.ViewModels.Base;
using BookObserver.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    class BooksViewModel : ViewModel
    {
        private IUserDialog? _userDialog;
        private CreatorBookWindow? _creatorWindow;
        private EditorBookWindow? _editorWindow;

        public Dictionary<string, SortDescription> Sorting { get; } = new()
        {
            {"Сначала старые записи", new SortDescription("Id", ListSortDirection.Ascending)},
            {"Сначала новые записи", new SortDescription("Id", ListSortDirection.Descending)},
            {"Сначала в наличии", new SortDescription("Existence", ListSortDirection.Ascending)},
            {"Сначала не в наличии", new SortDescription("Existence", ListSortDirection.Descending)},
            {"ББК (по возрастанию)", new SortDescription("BBK", ListSortDirection.Ascending)},
            {"ББК (по убыванию)", new SortDescription("BBK", ListSortDirection.Descending)},
            {"Авторы (по возрастанию)", new SortDescription("Author", ListSortDirection.Ascending)},
            {"Авторы (по убыванию)", new SortDescription("Author", ListSortDirection.Descending)},
            {"Названия (по возрастанию)", new SortDescription("Name", ListSortDirection.Ascending)},
            {"Названия (по убыванию)", new SortDescription("Name", ListSortDirection.Descending)},
            {"Даты получения (по возрастанию)", new SortDescription("DateGet", ListSortDirection.Ascending)},
            {"Даты получения (по убыванию)", new SortDescription("DateGet", ListSortDirection.Descending)},
            {"Даты возврата (по возрастанию)", new SortDescription("DateSet", ListSortDirection.Ascending)},
            {"Даты возврата (по убыванию)", new SortDescription("DateSet", ListSortDirection.Descending)},
        };

        #region SelectedSorting : string - Выбранная сортировка списка книг

        ///<summary>Выбранная сортировка списка книг</summary>
        private string _selectedSorting = "Сначала старые записи";

        ///<summary>Выбранная сортировка списка книг</summary>
        public string SelectedSorting
        {
            get => _selectedSorting;
            set
            {
                if (!Set(ref _selectedSorting, value)) return;

                _booksView.View.SortDescriptions.Clear();
                ClearGarbage();
                if (value.Contains("Даты получения", StringComparison.OrdinalIgnoreCase)
                    || value.Contains("Даты возврата", StringComparison.OrdinalIgnoreCase))
                    _booksView.View.SortDescriptions.Add(Sorting["Сначала не в наличии"]);
                _booksView.View.SortDescriptions.Add(Sorting[value]);
            }
        }

        #endregion

        ///<summary>Список книг</summary>
        public ObservableCollection<Book> Books { get; }

        #region FiltredBooks : ObservableCollection<Book> - Список книг для фильтрации

        private ObservableCollection<Book>? _filtredBooks;
        ///<summary>Список книг для фильтрации</summary>
        public ObservableCollection<Book>? FiltredBooks { get => _filtredBooks; set => Set(ref _filtredBooks, value); }

        #endregion
        /// <summary>Вывод списка</summary>
        public readonly CollectionViewSource _booksView = new();
        /// <summary>Вывод списка</summary>
        public ICollectionView BooksView => _booksView.View;

        #region SelectedBook : Book? - Выбранная книга

        ///<summary>Выбранная книга</summary>
        private Book? _selectedBook;

        ///<summary>Выбранная книга</summary>
        public Book? SelectedBook { get => _selectedBook; set => Set(ref _selectedBook, value); }

        #endregion

        private readonly CollectionWithFilter _existencesView = new();
        /// <summary>Вывод списка "в наличии"</summary>
        public ObservableCollection<string>? ExistencesView => _existencesView.CollectionView;

        #region SelectedExistence : string - Выбранное наличие книги

        ///<summary>Выбранное наличие книги</summary>
        private string _selectedExistence = string.Empty;

        ///<summary>Выбранное наличие книги</summary>
        public string SelectedExistence
        {
            get => _selectedExistence;
            set
            {
                if (!Set(ref _selectedExistence, value)) return;

                _existencesView.RefreshFilter(value);
                OnPropertyChanged(nameof(ExistencesView));
                ((Command)FindBooksCommand).Executable = true;
            }
        }

        #endregion

        private readonly CollectionWithFilter _bbksView = new();
        /// <summary>Вывод списка ББК</summary>
        public ObservableCollection<string>? BBKsView => _bbksView.CollectionView;

        #region SelectedBBK : string - Выбранный ББК

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
                ((Command)FindBooksCommand).Executable = true;
            }
        }

        #endregion

        private readonly CollectionWithFilter _authorsView = new();
        /// <summary>Вывод списка авторов</summary>
        public ObservableCollection<string>? AuthorsView => _authorsView.CollectionView;

        #region SelectedAuthor : string - Выбранный автор

        ///<summary>Выбранный автор</summary>
        private string _authorsFilterText = string.Empty;

        ///<summary>Выбранный автор</summary>
        public string SelectedAuthor
        {
            get => _authorsFilterText;
            set
            {
                if (!Set(ref _authorsFilterText, value)) return;

                _authorsView.RefreshFilter(value);
                OnPropertyChanged(nameof(AuthorsView));
                ((Command)FindBooksCommand).Executable = true;

            }
        }

        #endregion

        private readonly CollectionWithFilter _namesView = new();
        /// <summary>Вывод списка названий книг</summary>
        public ObservableCollection<string>? NamesView => _namesView.CollectionView;

        #region SelectedName : string - Выбранное название

        ///<summary>Выбранное название</summary>
        private string _selectedName = string.Empty;

        ///<summary>Выбранное название</summary>
        public string SelectedName
        {
            get => _selectedName;
            set
            {
                if (!Set(ref _selectedName, value)) return;

                _namesView.RefreshFilter(value);
                OnPropertyChanged(nameof(NamesView));
                ((Command)FindBooksCommand).Executable = true;
            }
        }

        #endregion

        private readonly CollectionWithFilter _publishesView = new();
        /// <summary>Вывод списка книг</summary>
        public ObservableCollection<string>? PublishesView => _publishesView.CollectionView;

        #region SelectedPublish : string - Выбранное издательство

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
                ((Command)FindBooksCommand).Executable = true;
            }
        }

        #endregion

        #region Commands...

        #region FindBooksCommand - Команда поиска книг

        ///<summary>Команда поиска книг</summary>
        private ICommand? _findBooksCommand;

        ///<summary>Команда поиска книг</summary>
        public ICommand FindBooksCommand => _findBooksCommand
            ??= new LambdaCommand(OnFindBooksCommandExecuted, CanFindBooksCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда поиска книг</summary>
        private bool CanFindBooksCommandExecute(object? p) =>
            p is not null
            && p is IList<Book>
            && (!string.IsNullOrWhiteSpace(_selectedExistence)
            || !string.IsNullOrWhiteSpace(_selectedBBK)
            || !string.IsNullOrWhiteSpace(_authorsFilterText)
            || !string.IsNullOrWhiteSpace(_selectedName)
            || !string.IsNullOrWhiteSpace(_selectedPublish)
            );

        ///<summary>Логика выполнения - Команда поиска книг</summary>
        private void OnFindBooksCommandExecuted(object? p)
        {
            IList<Book> result = (p as IList<Book>)!;
            if (!string.IsNullOrWhiteSpace(_selectedExistence)) result = result.Where(
                p => p.Existence.Contains(_selectedExistence, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_selectedBBK)) result = result.Where(
                p => p.BBK.Contains(_selectedBBK, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_authorsFilterText)) result = result.Where(
                p => p.Author.Contains(_authorsFilterText, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_selectedName)) result = result.Where(
                p => p.Name.Contains(_selectedName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_selectedPublish)) result = result.Where(
                p => p.Publish.Contains(_selectedPublish, StringComparison.OrdinalIgnoreCase)).ToList();
            FiltredBooks = null;
            _booksView.View.SortDescriptions.Clear();
            ClearGarbage();
            _booksView.Source = FiltredBooks = new(result);
            _booksView.View.SortDescriptions.Add(Sorting[_selectedSorting]);
            OnPropertyChanged(nameof(BooksView));
            ((Command)ResetToZeroFindCommand).Executable = true;
            ((Command)FindBooksCommand).Executable = false;
        }

        #endregion

        #region ClearFiltersCommand - Команда для очистки фильтров

        ///<summary>Команда для очистки фильтров</summary>
        private ICommand? _clearFiltersCommand;

        ///<summary>Команда для очистки фильтров</summary>
        public ICommand ClearFiltersCommand => _clearFiltersCommand
            ??= new LambdaCommand(OnClearFiltersCommandExecuted, CanClearFiltersCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда для очистки фильтров</summary>
        private bool CanClearFiltersCommandExecute(object? p) =>
            !string.IsNullOrWhiteSpace(_selectedExistence)
            || !string.IsNullOrWhiteSpace(_selectedBBK)
            || !string.IsNullOrWhiteSpace(_authorsFilterText)
            || !string.IsNullOrWhiteSpace(_selectedName)
            || !string.IsNullOrWhiteSpace(_selectedPublish)
            ;

        ///<summary>Логика выполнения - Команда для очистки фильтров</summary>
        private void OnClearFiltersCommandExecuted(object? p)
        {
            SelectedExistence = string.Empty;
            SelectedBBK = string.Empty;
            SelectedAuthor = string.Empty;
            SelectedName = string.Empty;
            SelectedPublish = string.Empty;
        }

        #endregion

        #region ResetToZeroFindCommand - Команда обнуления поиска книг

        ///<summary>Команда обнуления поиска книг</summary>
        private ICommand? _resetToZeroCommandCommand;

        ///<summary>Команда обнуления поиска книг</summary>
        public ICommand ResetToZeroFindCommand => _resetToZeroCommandCommand
            ??= new LambdaCommand(OnResetToZeroFindCommandExecuted, CanResetToZeroFindCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда обнуления поиска книг</summary>
        private bool CanResetToZeroFindCommandExecute(object? p) => p is not null
            && p is ObservableCollection<Book>;

        ///<summary>Логика выполнения - Команда обнуления поиска книг</summary>
        private void OnResetToZeroFindCommandExecuted(object? p)
        {
            _booksView.Source = p as ObservableCollection<Book>;
            OnPropertyChanged(nameof(BooksView));
            ((Command)ResetToZeroFindCommand).Executable = false;
            ((Command)FindBooksCommand).Executable = true;
        }

        #endregion

        #region DeleteBookCommand - Команда удаления книги

        ///<summary>Команда удаления книги</summary>
        private ICommand? _deleteBookCommand;

        ///<summary>Команда удаления книги</summary>
        public ICommand DeleteBookCommand => _deleteBookCommand
            ??= new LambdaCommand(OnDeleteBookCommandExecuted, CanDeleteBookCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда удаления книги</summary>
        private bool CanDeleteBookCommandExecute(object? p) =>
            Books is not null
            && p is not null
            && p is Book;

        ///<summary>Логика выполнения - Команда удаления книги</summary>
        private void OnDeleteBookCommandExecuted(object? p)
        {
            Book book = (p as Book)!;
            var index = Books.IndexOf(book);
            if (Books.Remove(book))
                for (int i = index; i < Books.Count; i++)
                    Books[i].Id--;
            FiltredBooks?.Remove(book);
            ((Command)SaveBooksCommand).Executable = true;
        }

        #endregion

        #region SaveBooksCommand - Команда сохранения книг в файл

        ///<summary>Команда сохранения книг в файл</summary>
        private ICommand? _saveBooksCommand;

        ///<summary>Команда сохранения книг в файл</summary>
        public ICommand SaveBooksCommand => _saveBooksCommand
            ??= new LambdaCommand(OnSaveBooksCommandExecuted, CanSaveBooksCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда сохранения книг в файл</summary>
        private bool CanSaveBooksCommandExecute(object? p) => p is not null
            && p is IList<Book>;

        ///<summary>Логика выполнения - Команда сохранения книг в файл</summary>
        private async void OnSaveBooksCommandExecuted(object? p)
        {
            using (var writer = new StreamWriter($@"{Environment.CurrentDirectory}/Data/Books.json"))
                await writer.WriteAsync(JsonConvert.SerializeObject(p, Formatting.Indented));
            ((Command)SaveBooksCommand).Executable = false;
            _userDialog?.ShowInformation("Файл успешно сохранен", "BookObserver");
            ((Command)SaveBooksCommand).Executable = false;
        }

        #endregion

        #region AddBookCommand - Команда добавить книгу

        ///<summary>Команда добавить книгу</summary>
        private ICommand? _addBookCommand;

        ///<summary>Команда добавить книгу</summary>
        public ICommand AddBookCommand => _addBookCommand
            ??= new LambdaCommand(OnAddBookCommandExecuted, CanAddBookCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда добавить книгу</summary>
        private bool CanAddBookCommandExecute(object? p) => true;

        ///<summary>Логика выполнения - Команда добавить книгу</summary>
        private void OnAddBookCommandExecuted(object? p)
        {
            if (_creatorWindow is { } window)
            {
                window.Show();
                return;
            }
            window = App.Services.GetRequiredService<CreatorBookWindow>();
            window.Closed += (_, _) =>
            {
                _creatorWindow = null;
                ClearGarbage();
            };
            _creatorWindow = window;
            window.Show();
            ((Command)SaveBooksCommand).Executable = true;

        }

        #endregion

        #region EditBookCommand - Команда редактирования книги

        ///<summary>Команда редактирования книги</summary>
        private ICommand? _editBookCommand;

        ///<summary>Команда редактирования книги</summary>
        public ICommand EditBookCommand => _editBookCommand
            ??= new LambdaCommand(OnEditBookCommandExecuted, CanEditBookCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда редактирования книги</summary>
        private bool CanEditBookCommandExecute(object? p) => _selectedBook is not null;

        ///<summary>Логика выполнения - Команда редактирования книги</summary>
        private void OnEditBookCommandExecuted(object? p)
        {
            if (_editorWindow is { } window)
            {
                _editorWindow.ShowDialog();
                return;
            }
            window = App.Services.GetRequiredService<EditorBookWindow>();
            window.Closed += (_, _) =>
            {
                _editorWindow = null;
                ClearGarbage();
            };
            _editorWindow = window;
            if (window.ShowDialog() == true)
                ((Command)SaveBooksCommand).Executable = true;
        }

        #endregion

        #region GotFocus (Commands)...

        #region GotFocusComboBoxExistencesCommand - Команда при получении фокуса (ComboBox "в наличии")

        ///<summary>Команда при получении фокуса (В наличии)</summary>
        private ICommand? _gotFocusComboBoxExistencesCommand;

        ///<summary>Команда при получении фокуса (В наличии)</summary>
        public ICommand GotFocusComboBoxExistencesCommand => _gotFocusComboBoxExistencesCommand
            ??= new LambdaCommand(OnGotFocusComboBoxExistencesCommandExecuted, CanGotFocusComboBoxExistencesCommandExecute);

        private bool CanGotFocusComboBoxExistencesCommandExecute(object? p) =>
            p is not null && p is IList<Book>;

        ///<summary>Логика выполнения - Команда при получении фокуса (В наличии)</summary>
        private void OnGotFocusComboBoxExistencesCommandExecuted(object? p)
        {
            _existencesView.List = (p as IList<Book>)!
                .Select(b => b.Existence)
                .Distinct()
                .Order()
                .ToList()!;
            OnPropertyChanged(nameof(ExistencesView));
        }

        #endregion

        #region GotFocusComboBoxBBKssCommand - Команда при получении фокуса (ComboBox ББК)

        ///<summary>Команда при получении фокуса (ComboBox ББК)</summary>
        private ICommand? _gotFocusComboBoxBBKsCommand;

        ///<summary>Команда при получении фокуса (ComboBox ББК)</summary>
        public ICommand GotFocusComboBoxBBKsCommand => _gotFocusComboBoxBBKsCommand
            ??= new LambdaCommand(OnGotFocusComboBoxBBKsCommandExecuted, CanGotFocusComboBoxBBKsCommandExecute);

        private bool CanGotFocusComboBoxBBKsCommandExecute(object? p) =>
            p is not null && p is IList<Book>;

        ///<summary>Логика выполнения - Команда при получении фокуса (ComboBox ББК)</summary>
        private void OnGotFocusComboBoxBBKsCommandExecuted(object? p)
        {
            _bbksView.List = (p as IList<Book>)!
                .Select(b => b.BBK)
                .Distinct()
                .Order()
                .ToList()!;
            OnPropertyChanged(nameof(BBKsView));
        }

        #endregion

        #region GotFocusComboBoxAuthorsCommand - Команда при получении фокуса (ComboBox авторов)

        ///<summary>Команда при получении фокуса (ComboBox авторов)</summary>
        private ICommand? _gotFocusComboBoxAuthorsCommand;

        ///<summary>Команда при получении фокуса (ComboBox авторов)</summary>
        public ICommand GotFocusComboBoxAuthorsCommand => _gotFocusComboBoxAuthorsCommand
            ??= new LambdaCommand(OnGotFocusComboBoxAuthorsCommandExecuted, CanGotFocusComboBoxAuthorsCommandExecute);

        private bool CanGotFocusComboBoxAuthorsCommandExecute(object? p) =>
            p is not null && p is IList<Book>;

        ///<summary>Логика выполнения - Команда при получении фокуса (ComboBox авторов)</summary>
        private void OnGotFocusComboBoxAuthorsCommandExecuted(object? p)
        {
            _authorsView.List = (p as IList<Book>)!
                .Select(r => r.Author)
                .Distinct()
                .Order()
                .ToList()!;
            OnPropertyChanged(nameof(AuthorsView));
        }

        #endregion

        #region GotFocusComboBoxNamesCommand - Команда при получении фокуса (ComboBox названий)

        ///<summary>Команда при получении фокуса (ComboBox названий)</summary>
        private ICommand? _gotFocusComboBoxNamesCommand;

        ///<summary>Команда при получении фокуса (ComboBox названий)</summary>
        public ICommand GotFocusComboBoxNamesCommand => _gotFocusComboBoxNamesCommand
            ??= new LambdaCommand(OnGotFocusComboBoxNamesCommandExecuted, CanGotFocusComboBoxNamesCommandExecute);

        private bool CanGotFocusComboBoxNamesCommandExecute(object? p) =>
            p is not null && p is IList<Book>;

        ///<summary>Логика выполнения - Команда при получении фокуса (ComboBox названий)</summary>
        private void OnGotFocusComboBoxNamesCommandExecuted(object? p)
        {
            _namesView.List = (p as IList<Book>)!
                .Select(r => r.Name)
                .Distinct()
                .Order()
                .ToList()!;
            OnPropertyChanged(nameof(NamesView));
        }

        #endregion

        #region GotFocusComboBoxPublishesCommand - Команда при получении фокуса (ComboBox издательств)

        ///<summary>Команда при получении фокуса (ComboBox издательств)</summary>
        private ICommand? _GotFocusComboBoxPublishesCommand;

        ///<summary>Команда при получении фокуса (ComboBox издательств)</summary>
        public ICommand GotFocusComboBoxPublishesCommand => _GotFocusComboBoxPublishesCommand
            ??= new LambdaCommand(OnGotFocusComboBoxPublishesCommandExecuted, CanGotFocusComboBoxPublishesCommandExecute);

        private bool CanGotFocusComboBoxPublishesCommandExecute(object? p) =>
            p is not null && p is IList<Book>;

        ///<summary>Логика выполнения - Команда при получении фокуса (ComboBox издательств)</summary>
        private void OnGotFocusComboBoxPublishesCommandExecuted(object? p)
        {
            _publishesView.List = (p as IList<Book>)!
                .Select(r => r.Publish)
                .Distinct()
                .Order()
                .ToList()!;
            OnPropertyChanged(nameof(PublishesView));
        }

        #endregion

        #endregion

        #endregion

        public BooksViewModel(IUserDialog userDialog)
        {
            _userDialog = userDialog;
            try
            {
                using (var reader = new StreamReader($@"{Environment.CurrentDirectory}/Data/Books.json"))
                {
                    Books = JsonConvert.DeserializeObject<ObservableCollection<Book>>(reader.ReadToEnd());
                }
                Books ??= [];
            }
            catch (Exception)
            {
                Books = [];
            }
            finally
            {
                _booksView.Source = FiltredBooks = Books;
                ((Command)ResetToZeroFindCommand).Executable = false;
                ((Command)SaveBooksCommand).Executable = false;
            }
        }
    }
}
