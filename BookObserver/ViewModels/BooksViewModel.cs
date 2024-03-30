using BookObserver.Infrastructure.Commands;
using BookObserver.Infrastructure.Commands.Base;
using BookObserver.Models.Books;
using BookObserver.ViewModels.Base;
using BookObserver.Views.Windows;
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
        #region Properties

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
            {"Названия (по убыванию)", new SortDescription("Name", ListSortDirection.Descending)}
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
                _booksView.View.SortDescriptions.Add(Sorting[value]);
            }
        }

        #endregion

        ///<summary>Список книг</summary>
        public ObservableCollection<Book> Books { get; }

        #region FiltredBooks : ObservableCollection<Book> - Список книг для фильтрации

        ///<summary>Список книг для фильтрации</summary>
        private ObservableCollection<Book>? _filtredBooks;

        ///<summary>Список книг для фильтрации</summary>
        public ObservableCollection<Book>? FiltredBooks { get => _filtredBooks; set => Set(ref _filtredBooks, value); }

        #endregion

        #region BooksView : ICollectionView - Вывод списка книг

        public readonly CollectionViewSource _booksView = new();
        public ICollectionView BooksView => _booksView.View;

        #endregion

        #region Existences : IList<string> - Список "в наличии"

        ///<summary>Список "в наличии"</summary>
        private IList<string> _existences = [];

        ///<summary>Список "в наличии"</summary>
        public IList<string> Existences
        {
            get => _existences;
            private set => Set(ref _existences, value);
        }

        #endregion

        #region FiltredExistences : ObservableCollection<string>? - Список "в наличии" для фильтрации

        ///<summary>Список "в наличии" для фильтрации</summary>
        private ObservableCollection<string>? _filtredExistences;

        ///<summary>Список "в наличии" для фильтрации</summary>
        public ObservableCollection<string>? FiltredExistences
        {
            get => _filtredExistences;
            private set => Set(ref _filtredExistences, value);
        }

        #endregion

        /// <summary>Вывод списка "в наличии"</summary>
        public ObservableCollection<string>? ExistencesView => _filtredExistences;

        #region SelectedExistence : string? - Выбранное наличие книги

        ///<summary>Выбранное наличие книги</summary>
        private string? _selectedExistence;

        ///<summary>Выбранное наличие книги</summary>
        public string? SelectedExistence
        {
            get => _selectedExistence;
            set
            {
                if (!Set(ref _selectedExistence, value)) return;

                if (!((Command)FindBooksCommand).Executable)
                    ((Command)FindBooksCommand).Executable = true;
                if (value is not null)
                {
                    FiltredExistences = null;
                    ClearGarbage();
                    FiltredExistences = new(_existences.Where(s => s.Contains(value, StringComparison.OrdinalIgnoreCase)));
                    OnPropertyChanged(nameof(ExistencesView));
                    ExecutableCommandFindBooksCommandChange();
                }
            }
        }

        #endregion

        #region BBKs : IList<string> - Список ББК

        ///<summary>Список ББК</summary>
        private IList<string> _bbks = [];

        ///<summary>Список ББК</summary>
        public IList<string> BBKs
        {
            get => _bbks;
            private set => Set(ref _bbks, value);
        }

        #endregion

        #region FiltredBBK : ObservableCollection<string>? - Список ББК для фильтрации

        ///<summary>Список ББК для фильтрации</summary>
        private ObservableCollection<string>? _filtredBBKs;

        ///<summary>Список ББК для фильтрации</summary>
        public ObservableCollection<string>? FiltredBBKs { get => _filtredBBKs; set => Set(ref _filtredBBKs, value); }

        #endregion

        /// <summary>Вывод списка ББК</summary>
        public ObservableCollection<string>? BBKsView => _filtredBBKs;

        #region SelectedBBK : string? - Выбранный ББК

        ///<summary>Выбранный ББК</summary>
        private string? _selectedBBK;

        ///<summary>Выбранный ББК</summary>
        public string? SelectedBBK
        {
            get => _selectedBBK;
            set
            {
                if (!Set(ref _selectedBBK, value)) return;
                if (!((Command)FindBooksCommand).Executable)
                    ((Command)FindBooksCommand).Executable = true;
                if (value is not null)
                {
                    FiltredBBKs = null;
                    ClearGarbage();
                    FiltredBBKs = new(_bbks.Where(bbk => bbk.Contains(value, StringComparison.OrdinalIgnoreCase)));
                    OnPropertyChanged(nameof(BBKsView));
                    ExecutableCommandFindBooksCommandChange();
                }
            }
        }

        #endregion

        #region Authors : IList<string> - Список авторов

        ///<summary>Список авторов</summary>
        private IList<string> _authors = [];

        ///<summary>Список авторов</summary>
        public IList<string> Authors
        {
            get => _authors;
            private set => Set(ref _authors, value);
        }

        #endregion

        #region FiltredAuthors : ObservableCollection<string>? - Список авторов для фильтрации

        ///<summary>Список авторов для фильтрации</summary>
        private ObservableCollection<string>? _filtredAuthors;

        ///<summary>Список авторов для фильтрации</summary>
        public ObservableCollection<string>? FiltredAuthors
        {
            get => _filtredAuthors;
            private set => Set(ref _filtredAuthors, value);
        }

        #endregion

        /// <summary>Вывод списка авторов</summary>
        public ObservableCollection<string>? AuthorsView => _filtredAuthors;

        #region SelectedAuthor : string? - Выбранный автор

        ///<summary>Выбранный автор</summary>
        private string? _authorsFilterText;

        ///<summary>Выбранный автор</summary>
        public string? SelectedAuthor
        {
            get => _authorsFilterText;
            set
            {
                if (!Set(ref _authorsFilterText, value)) return;

                if (!((Command)FindBooksCommand).Executable)
                    ((Command)FindBooksCommand).Executable = true;
                if (value is not null)
                {
                    FiltredAuthors = null;
                    ClearGarbage();
                    FiltredAuthors = new(_authors.Where(a => a.Contains(value, StringComparison.OrdinalIgnoreCase)));
                    OnPropertyChanged(nameof(AuthorsView));
                    ExecutableCommandFindBooksCommandChange();
                }
            }
        }

        #endregion

        #region Names : List<string> - Список названий книг

        ///<summary>Список названий книг</summary>
        private List<string> _names = [];

        ///<summary>Список названий книг</summary>
        public List<string> Names
        {
            get => _names;
            private set => Set(ref _names, value);
        }

        #endregion

        #region FiltredNames : ObservableCollection<string>? - Список названий книг для фильтрации

        ///<summary>Список названий книг для фильтрации</summary>
        private ObservableCollection<string>? _filtredNames;

        ///<summary>Список названий книг для фильтрации</summary>
        public ObservableCollection<string>? FiltredNames
        {
            get => _filtredNames;
            private set => Set(ref _filtredNames, value);
        }

        #endregion

        public ObservableCollection<string>? NamesView => _filtredNames;

        #region SelectedName : string? - Выбранное название

        ///<summary>Выбранное название</summary>
        private string? _selectedName;

        ///<summary>Выбранное название</summary>
        public string? SelectedName
        {
            get => _selectedName;
            set
            {
                if (!Set(ref _selectedName, value)) return;

                if (!((Command)FindBooksCommand).Executable)
                    ((Command)FindBooksCommand).Executable = true;
                if (value is not null)
                {
                    FiltredNames = null;
                    ClearGarbage();
                    FiltredNames = new(_names.Where(n => n.Contains(value, StringComparison.OrdinalIgnoreCase)));
                    OnPropertyChanged(nameof(NamesView));
                    ExecutableCommandFindBooksCommandChange();
                }
            }
        }

        #endregion

        #region Publishes : IList<string> - Список издательств

        ///<summary>Список издательств</summary>
        private IList<string> _publishes = [];

        ///<summary>Список издательств</summary>
        public IList<string> Publishes
        {
            get => _publishes;
            private set => Set(ref _publishes, value);
        }

        #endregion

        #region FiltredPublishes : ObservableCollection<string>? - Список издательств для фильтрации

        ///<summary>Список издательств для фильтрации</summary>
        private ObservableCollection<string>? _filtredPublishes;

        ///<summary>Список издательств для фильтрации</summary>
        public ObservableCollection<string>? FiltredPublishes
        {
            get => _filtredPublishes;
            private set => Set(ref _filtredPublishes, value);
        }

        #endregion

        /// <summary>Вывод списка книг</summary>
        public ObservableCollection<string>? PublishesView => _filtredPublishes;

        #region SelectedPublish : string? - Выбранное издательство

        ///<summary>Выбранное издательство</summary>
        private string? _selectedPublish;

        ///<summary>Выбранное издательство</summary>
        public string? SelectedPublish
        {
            get => _selectedPublish;
            set
            {
                if (!Set(ref _selectedPublish, value)) return;

                if (value is not null)
                {
                    FiltredPublishes = null;
                    ClearGarbage();
                    FiltredPublishes = new(_publishes.Where(p => p.Contains(value, StringComparison.OrdinalIgnoreCase)));
                    OnPropertyChanged(nameof(PublishesView));
                    ExecutableCommandFindBooksCommandChange();
                }
            }
        }

        #endregion

        #region SelectedBook : Book? - Выбранная книга

        ///<summary>Выбранная книга</summary>
        private Book? _selectedBook;

        ///<summary>Выбранная книга</summary>
        public Book? SelectedBook { get => _selectedBook; set => Set(ref _selectedBook, value); }

        #endregion

        #endregion

        #region Commands

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
                p => p.Existence!.Contains(_selectedExistence, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_selectedBBK)) result = result.Where(
                p => p.BBK!.Contains(_selectedBBK, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_authorsFilterText)) result = result.Where(
                p => p.Author!.Contains(_authorsFilterText, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_selectedName)) result = result.Where(
                p => p.Name!.Contains(_selectedName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_selectedPublish)) result = result.Where(
                p => p.Publish!.Contains(_selectedPublish, StringComparison.OrdinalIgnoreCase)).ToList();
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
            SelectedExistence = null;
            SelectedBBK = null;
            SelectedAuthor = null;
            SelectedName = null;
            SelectedPublish = null;
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
            if (!((Command)FindBooksCommand).Executable)
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
            Books.Remove((p as Book)!);
            FiltredBooks?.Remove((p as Book)!);
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
            {
                await writer.WriteAsync(JsonConvert.SerializeObject(p, Formatting.Indented));
            }
            ((Command)SaveBooksCommand).Executable = false;
            MessageBox.Show("Файл успешно сохранен");
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
            var model = new Creator_EditorBookViewModel(this);
            var window = new Creator_EditorBookWindow { DataContext = model };
            window.Closed += (_, _) => window.DataContext = null;
            window.ShowDialog();
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
            Existences = (p as IList<Book>)!
                .Select(b => b.Existence)
                .Distinct()
                .Order()
                .ToList()!;
            FiltredExistences = new(_existences);
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
            BBKs = (p as IList<Book>)!
                .Select(b => b.BBK)
                .Distinct()
                .Order()
                .ToList()!;
            FiltredBBKs = new(_bbks);
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
            Authors = (p as IList<Book>)!
                .Select(r => r.Author)
                .Distinct()
                .Order()
                .ToList()!;
            FiltredAuthors = new(_authors);
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
            Names = (p as IList<Book>)!
                .Select(r => r.Name)
                .Distinct()
                .Order()
                .ToList()!;
            FiltredNames = new(_names);
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
            Publishes = (p as IList<Book>)!
                .Select(r => r.Publish)
                .Distinct()
                .Order()
                .ToList()!;
            FiltredPublishes = new(_publishes);
            OnPropertyChanged(nameof(PublishesView));
        }

        #endregion

        #endregion

        #region LostFocus (Commands)...

        #region LostFocusComboBoxExtensiencesCommand - Команда при потере фокуса (В наличии)

        ///<summary>Команда при потере фокуса (В наличии)</summary>
        private ICommand? _LostFocusComboBoxExtensiencesCommand;

        ///<summary>Команда при потере фокуса (В наличии)</summary>
        public ICommand LostFocusComboBoxExistencesCommand => _LostFocusComboBoxExtensiencesCommand
            ??= new LambdaCommand(OnLostFocusComboBoxExtensiencesCommandExecuted);

        ///<summary>Логика выполнения - Команда при потере фокуса (В наличии)</summary>
        private void OnLostFocusComboBoxExtensiencesCommandExecuted(object? p)
        {
            FiltredExistences = null;
            ClearGarbage();
        }

        #endregion

        #region LostFocusComboBoxBBKsCommand - Команда при потере фокуса (ComboBox ББК)

        ///<summary>Команда при потере фокуса (ComboBox ББК)</summary>
        private ICommand? _LostFocusComboBoxBBKsCommand;

        ///<summary>Команда при потере фокуса (ComboBox ББК)</summary>
        public ICommand LostFocusComboBoxBBKsCommand => _LostFocusComboBoxBBKsCommand
            ??= new LambdaCommand(OnLostFocusComboBoxBBKsCommandExecuted);

        ///<summary>Логика выполнения - Команда при потере фокуса (ComboBox ББК)</summary>
        private void OnLostFocusComboBoxBBKsCommandExecuted(object? p)
        {
            FiltredBBKs = null;
            ClearGarbage();
        }

        #endregion

        #region LostFocusComboBoxAuthorsCommand - Команда при потере фокуса (ComboBox авторов)

        ///<summary>Команда при потере фокуса (ComboBox авторов)</summary>
        private ICommand? _LostFocusComboBoxAuthorsCommand;

        ///<summary>Команда при потере фокуса (ComboBox авторов)</summary>
        public ICommand LostFocusComboBoxAuthorsCommand => _LostFocusComboBoxAuthorsCommand
            ??= new LambdaCommand(OnLostFocusComboBoxAuthorsCommandExecuted);

        ///<summary>Логика выполнения - Команда при потере фокуса (ComboBox авторов)</summary>
        private void OnLostFocusComboBoxAuthorsCommandExecuted(object? p)
        {
            FiltredAuthors = null;
            ClearGarbage();
        }

        #endregion

        #region LostFocusComboBoxNamesCommand - Команда при потере фокуса (ComboBox названий)

        ///<summary>Команда при потере фокуса (ComboBox названий)</summary>
        private ICommand? _LostFocusComboBoxNamesCommand;

        ///<summary>Команда при потере фокуса (ComboBox названий)</summary>
        public ICommand LostFocusComboBoxNamesCommand => _LostFocusComboBoxNamesCommand
            ??= new LambdaCommand(OnLostFocusComboBoxNamesCommandExecuted);

        ///<summary>Логика выполнения - Команда при потере фокуса (ComboBox названий)</summary>
        private void OnLostFocusComboBoxNamesCommandExecuted(object? p)
        {
            FiltredNames = null;
            ClearGarbage();
        }

        #endregion

        #region LostFocusComboBoxPublishesCommand - Команда при потере фокуса (ComboBox издательств)

        ///<summary>Команда при потере фокуса (ComboBox издательств)</summary>
        private ICommand? _lostFocusComboBoxPublishesCommand;

        ///<summary>Команда при потере фокуса (ComboBox издательств)</summary>
        public ICommand LostFocusComboBoxPublishesCommand => _lostFocusComboBoxPublishesCommand
            ??= new LambdaCommand(OnLostFocusComboBoxPublishesCommandExecuted);

        ///<summary>Логика выполнения - Команда при потере фокуса (ComboBox издательств)</summary>
        private void OnLostFocusComboBoxPublishesCommandExecuted(object? p)
        {
            FiltredPublishes = null;
            ClearGarbage();
        }

        #endregion

        #endregion

        #endregion

        public BooksViewModel()
        {
            Books = new(Enumerable.Range(1, 100000).Select(p => new Book
            {
                Id = p,
                BBK = Random.Shared.Next(0, 100).ToString(),
                Pages = p + Random.Shared.Next(0, 100),
                Author = $"Author {p}",
                Name = new string('ü', Random.Shared.Next(15, 30)),
                Reader = new Models.Readers.Reader
                {
                    FirstName = "Амплитуда"
                },
                Publish = $"Publish {p}",
                YearPublish = $"{p}",
                Existence = Random.Shared.Next(0, 2) == 0 ? "Да" : "Нет"
            }).ToList());
            _booksView.Source = FiltredBooks = Books;
            ((Command)ResetToZeroFindCommand).Executable = false;
        }

        private void ExecutableCommandFindBooksCommandChange()
        {
            if (!((Command)FindBooksCommand).Executable)
                ((Command)FindBooksCommand).Executable = true;
        }
    }
}
