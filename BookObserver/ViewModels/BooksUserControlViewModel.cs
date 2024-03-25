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
    class BooksUserControlViewModel : ViewModel
    {
        #region Properties

        ///<summary>Список книг</summary>
        public ObservableCollection<Book> Books { get; }

        #region BooksView : ICollectionView - Вывод списка книг

        public readonly CollectionViewSource _booksView = new();
        public ICollectionView BooksView => _booksView.View;

        #endregion

        #region StockView : ICollectionView - Вывод списка "в наличии"
        private readonly CollectionViewSource _stockView = new();
        public ICollectionView StockView => _stockView.View;
        #endregion

        #region AuthorsView : ICollectionView - Вывод списка авторов

        private readonly CollectionViewSource _authorsView = new();
        public ICollectionView AuthorsView => _authorsView.View;

        #endregion

        #region NamesView : ICollectionView - Вывод списка названий книг
        private readonly CollectionViewSource _namesView = new();
        public ICollectionView NamesView => _namesView.View;
        #endregion

        #region BBKView : ICollectionView - Вывод списка BBK
        private readonly CollectionViewSource _bbkView = new();
        public ICollectionView BBKView => _bbkView.View;
        #endregion

        #region PublishView : ICollectionView - Вывод списка издательства
        private readonly CollectionViewSource _publishView = new();
        public ICollectionView PublishView => _publishView.View;
        #endregion

        #region YearPublishView : ICollectionView - Вывод списка года издательств
        private readonly CollectionViewSource _yearPublishView = new();
        public ICollectionView YearPublishView => _yearPublishView.View;
        #endregion

        #region BooksFilterText : string? - Фильтр книг

        ///<summary>Фильтр книг</summary>
        private string? _booksFilterText;

        ///<summary>Фильтр книг</summary>
        public string? BooksFilterText
        {
            get => _booksFilterText;
            set
            {
                if (!Set(ref _booksFilterText, value)) return;

                if (!((Command)FindBooksCommand).Executable)
                    ((Command)FindBooksCommand).Executable = true;
                _booksView.View.Refresh();
            }
        }

        #endregion

        #region SelectedStock : string? - Выбранное наличие книги

        ///<summary>Выбранное наличие книги</summary>
        private string? _selectedStock;

        ///<summary>Выбранное наличие книги</summary>
        public string? SelectedStock
        {
            get => _selectedStock;
            set
            {
                if (!Set(ref _selectedStock, value)) return;

                if (!((Command)FindBooksCommand).Executable)
                    ((Command)FindBooksCommand).Executable = true;
                _stockView.View.Refresh();
            }
        }

        #endregion

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
                _bbkView.View.Refresh();
            }
        }

        #endregion

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
                _namesView.View.Refresh();
            }
        }

        #endregion

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
                _authorsView.View.Refresh();
            }
        }

        #endregion

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

                if (!((Command)FindBooksCommand).Executable)
                    ((Command)FindBooksCommand).Executable = true;
                _publishView.View.Refresh();
            }
        }

        #endregion

        #region SelectedYearPublish : string? - Выбранный год издания

        ///<summary>Выбранный год издания</summary>
        private string? _selectedYearPublish;

        ///<summary>Выбранный год издания</summary>
        public string? SelectedYearPublish
        {
            get => _selectedYearPublish;
            set
            {
                if (!Set(ref _selectedYearPublish, value)) return;

                if (!((Command)FindBooksCommand).Executable)
                    ((Command)FindBooksCommand).Executable = true;
                _yearPublishView.View.Refresh();
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

        #region FindBooksCommand - Поиск книг

        ///<summary>Поиск книг</summary>
        private ICommand? _findBooksCommand;

        ///<summary>Поиск книг</summary>
        public ICommand FindBooksCommand => _findBooksCommand
            ??= new LambdaCommand(OnFindBooksCommandExecuted, CanFindBooksCommandExecute);

        ///<summary>Проверка возможности выполнения - Поиск книг</summary>
        private bool CanFindBooksCommandExecute(object? p) =>
            p is not null
            && p is IList<Book>
            && (!string.IsNullOrWhiteSpace(_selectedStock)
            || !string.IsNullOrWhiteSpace(_selectedBBK)
            || !string.IsNullOrWhiteSpace(_authorsFilterText)
            || !string.IsNullOrWhiteSpace(_selectedName)
            || !string.IsNullOrWhiteSpace(_selectedPublish)
            || !string.IsNullOrWhiteSpace(_selectedYearPublish)
            );

        ///<summary>Логика выполнения - Поиск книг</summary>
        private void OnFindBooksCommandExecuted(object? p)
        {
            IList<Book> result = (p as IList<Book>)!;
            if (!string.IsNullOrWhiteSpace(_selectedStock)) result = result.Where(
                p => p.Stock.Contains(_selectedStock, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_selectedBBK)) result = result.Where(
                p => p.BBK.Contains(_selectedBBK, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_authorsFilterText)) result = result.Where(
                p => p.Author.Contains(_authorsFilterText, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_selectedName)) result = result.Where(
                p => p.Name.Contains(_selectedName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_selectedPublish)) result = result.Where(
                p => p.Publish.Contains(_selectedPublish, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_selectedYearPublish)) result = result.Where(
                p => Convert.ToString(p.YearPublish)
                .Contains(_selectedYearPublish, StringComparison.OrdinalIgnoreCase)).ToList();

            _booksView.Source = result;
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
            !string.IsNullOrWhiteSpace(_selectedStock)
            || !string.IsNullOrWhiteSpace(_selectedBBK)
            || !string.IsNullOrWhiteSpace(_authorsFilterText)
            || !string.IsNullOrWhiteSpace(_selectedName)
            || !string.IsNullOrWhiteSpace(_selectedPublish)
            || !string.IsNullOrWhiteSpace(_selectedYearPublish);

        ///<summary>Логика выполнения - Команда для очистки фильтров</summary>
        private void OnClearFiltersCommandExecuted(object? p)
        {
            SelectedStock = null;
            SelectedBBK = null;
            SelectedAuthor = null;
            SelectedName = null;
            SelectedPublish = null;
            SelectedYearPublish = null;
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
            Books?.Remove((p as Book)!);
            _booksView.Source = Books;
            OnPropertyChanged(nameof(BooksView));
            SelectedBook = null;
            SelectedStock = null;
            SelectedBBK = null;
            SelectedAuthor = null;
            SelectedName = null;
            SelectedPublish = null;
            SelectedYearPublish = null;
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

        #region MouseEnter (Commands)...

        #region MouseEnterComboBoxStockCommand - Команда когда курсор наводится на ComboBox (В наличии)

        ///<summary>Команда когда курсор наводится на ComboBox (В наличии)</summary>
        private ICommand? _mouseEnterComboBoxStockCommand;

        ///<summary>Команда когда курсор наводится на ComboBox (В наличии)</summary>
        public ICommand MouseEnterComboBoxStockCommand => _mouseEnterComboBoxStockCommand
            ??= new LambdaCommand(OnMouseEnterComboBoxStockCommandExecuted);

        ///<summary>Логика выполнения - Команда когда курсор наводится на ComboBox (В наличии)</summary>
        private void OnMouseEnterComboBoxStockCommandExecuted(object? p)
        {
            var value = _selectedStock;
            _stockView.Source = Books?.Select(book => book.Stock).Distinct().Order().ToList();
            OnPropertyChanged(nameof(StockView));
            _stockView.Filter += StockView_Filter;
            SelectedStock = value;
        }

        #endregion

        #region MouseEnterComboBoxBBKCommand - Команда когда курсор наводится на ComboBox (ББК)

        ///<summary>Команда когда курсор наводится на ComboBox (ББК)</summary>
        private ICommand? _mouseEnterComboBoxBBKCommand;

        ///<summary>Команда когда курсор наводится на ComboBox (ББК)</summary>
        public ICommand MouseEnterComboBoxBBKCommand => _mouseEnterComboBoxBBKCommand
            ??= new LambdaCommand(OnMouseEnterComboBoxBBKCommandExecuted);

        ///<summary>Логика выполнения - Команда когда курсор наводится на ComboBox (ББК)</summary>
        private void OnMouseEnterComboBoxBBKCommandExecuted(object? p)
        {
            var value = _selectedBBK;
            _bbkView.Source = Books?.Select(book => book.BBK).Distinct().Order().ToList();
            OnPropertyChanged(nameof(BBKView));
            _bbkView.Filter += BBKView_Filter;
            SelectedBBK = value;
        }

        #endregion

        #region MouseEnterComboBoxAuthorCommand - Команда когда курсор наводится на ComboBox (Автор)

        ///<summary>Команда когда курсор наводится на ComboBox (Автор)</summary>
        private ICommand? _mouseEnterComboBoxAuthorCommand;

        ///<summary>Команда когда курсор наводится на ComboBox (Автор)</summary>
        public ICommand MouseEnterComboBoxAuthorCommand => _mouseEnterComboBoxAuthorCommand
            ??= new LambdaCommand(OnMouseEnterComboBoxAuthorCommandExecuted);

        ///<summary>Логика выполнения - Команда когда курсор наводится на ComboBox (Автор)</summary>
        private void OnMouseEnterComboBoxAuthorCommandExecuted(object? p)
        {
            var value = _authorsFilterText;
            _authorsView.Source = Books?.Select(book => book.Author).Distinct().Order().ToList();
            OnPropertyChanged(nameof(AuthorsView));
            _authorsView.Filter += AuthorsView_Filter;
            SelectedAuthor = value;
        }

        #endregion

        #region MouseEnterComboBoxNameCommand - Команда когда курсор наводится на ComboBox (Название)

        ///<summary>Команда когда курсор наводится на ComboBox (Название)</summary>
        private ICommand? _mouseEnterComboBoxNameCommand;

        ///<summary>Команда когда курсор наводится на ComboBox (Название)</summary>
        public ICommand MouseEnterComboBoxNameCommand => _mouseEnterComboBoxNameCommand
            ??= new LambdaCommand(OnMouseEnterComboBoxNameCommandExecuted);

        ///<summary>Логика выполнения - Команда когда курсор наводится на ComboBox (Название)</summary>
        private void OnMouseEnterComboBoxNameCommandExecuted(object? p)
        {
            var value = _selectedName;
            _namesView.Source = Books?.Select(book => book.Name).Distinct().Order().ToList();
            OnPropertyChanged(nameof(NamesView));
            _namesView.Filter += NamesView_Filter;
            SelectedName = value;
        }

        #endregion

        #region MouseEnterComboBoxPublishCommand - Команда когда курсор наводится на ComboBox (Издательство)

        ///<summary>Команда когда курсор наводится на ComboBox (Издательство)</summary>
        private ICommand? _mouseEnterComboBoxPublishCommand;

        ///<summary>Команда когда курсор наводится на ComboBox (Издательство)</summary>
        public ICommand MouseEnterComboBoxPublishCommand => _mouseEnterComboBoxPublishCommand
            ??= new LambdaCommand(OnMouseEnterComboBoxPublishCommandExecuted);

        ///<summary>Логика выполнения - Команда когда курсор наводится на ComboBox (Издательство)</summary>
        private void OnMouseEnterComboBoxPublishCommandExecuted(object? p)
        {
            var value = _selectedPublish;
            _publishView.Source = Books?.Select(book => book.Publish).Distinct().Order().ToList();
            OnPropertyChanged(nameof(PublishView));
            _publishView.Filter += PublishView_Filter;
            SelectedPublish = value;
        }

        #endregion

        #region MouseEnterComboBoxYearPublishCommand - Команда когда курсор наводится на ComboBox (Год издательства)

        ///<summary>Команда когда курсор наводится на ComboBox (Год издательства)</summary>
        private ICommand? _mouseEnterComboBoxYearPublishCommand;

        ///<summary>Команда когда курсор наводится на ComboBox (Год издательства)</summary>
        public ICommand MouseEnterComboBoxYearPublishCommand => _mouseEnterComboBoxYearPublishCommand
            ??= new LambdaCommand(OnMouseEnterComboBoxYearPublishCommandExecuted);

        ///<summary>Логика выполнения - Команда когда курсор наводится на ComboBox (Год издательства)</summary>
        private void OnMouseEnterComboBoxYearPublishCommandExecuted(object? p)
        {
            var value = _selectedYearPublish;
            _yearPublishView.Source = Books?.Select(book => book.YearPublish).Distinct().Order().ToList();
            OnPropertyChanged(nameof(YearPublishView));
            _yearPublishView.Filter += YearPublishView_Filter;
            SelectedYearPublish = value;
        }

        #endregion

        #region MouseEnterTextBoxBookCommand - Команда когда курсор наводится на TextBox

        ///<summary>Команда когда курсор наводится на TextBox</summary>
        private ICommand? _mouseEnterTextBoxBookCommand;

        ///<summary>Команда когда курсор наводится на TextBox</summary>
        public ICommand MouseEnterTextBoxBookCommand => _mouseEnterTextBoxBookCommand
            ??= new LambdaCommand(OnMouseEnterTextBoxBookCommandExecuted);

        ///<summary>Логика выполнения - Команда когда курсор наводится на TextBox</summary>
        private void OnMouseEnterTextBoxBookCommandExecuted(object? p)
        {
            _booksView.Filter += BooksView_Filter;
        }

        #endregion

        #endregion

        #region MouseLeave (Commands)...

        #region MouseLeaveComboBoxStockCommand - Команда когда курсор выходит с ComboBox (В наличии)

        ///<summary>Команда когда курсор выходит с ComboBox (В наличии)</summary>
        private ICommand? _mouseLeaveComboBoxStockCommand;

        ///<summary>Команда когда курсор выходит с ComboBox (В наличии)</summary>
        public ICommand MouseLeaveComboBoxStockCommand => _mouseLeaveComboBoxStockCommand
            ??= new LambdaCommand(OnMouseLeaveComboBoxStockCommandExecuted);

        ///<summary>Логика выполнения - Команда когда курсор выходит с ComboBox (В наличии)</summary>
        private void OnMouseLeaveComboBoxStockCommandExecuted(object? p)
        {
            _stockView.Filter -= StockView_Filter;
            CallFinalizer();
        }

        #endregion

        #region MouseLeaveComboBoxBBKCommand - Команда когда курсор выходит с ComboBox (ББК)

        ///<summary>Команда когда курсор выходит с ComboBox (ББК)</summary>
        private ICommand? _mouseLeaveComboBoxBBKCommand;

        ///<summary>Команда когда курсор выходит с ComboBox (ББК)</summary>
        public ICommand MouseLeaveComboBoxBBKCommand => _mouseLeaveComboBoxBBKCommand
            ??= new LambdaCommand(OnMouseLeaveComboBoxBBKCommandExecuted);

        ///<summary>Логика выполнения - Команда когда курсор выходит с ComboBox (ББК)</summary>
        private void OnMouseLeaveComboBoxBBKCommandExecuted(object? p)
        {
            _bbkView.Filter -= BBKView_Filter;
            CallFinalizer();
        }

        #endregion

        #region MouseLeaveComboBoxAuthorCommand - Команда когда курсор выходит с ComboBox (Автор)

        ///<summary>Команда когда курсор выходит с ComboBox (Автор)</summary>
        private ICommand? _mouseLeaveComboBoxAuthorCommand;

        ///<summary>Команда когда курсор выходит с ComboBox (Автор)</summary>
        public ICommand MouseLeaveComboBoxAuthorCommand => _mouseLeaveComboBoxAuthorCommand
            ??= new LambdaCommand(OnMouseLeaveComboBoxAuthorCommandExecuted);

        ///<summary>Логика выполнения - Команда когда курсор выходит с ComboBox (Автор)</summary>
        private void OnMouseLeaveComboBoxAuthorCommandExecuted(object? p)
        {
            _authorsView.Filter -= AuthorsView_Filter;
            CallFinalizer();
        }

        #endregion

        #region MouseLeaveComboBoxNameCommand - Команда когда курсор выходит с ComboBox (Название)

        ///<summary>Команда когда курсор выходит с ComboBox (Название)</summary>
        private ICommand? _mouseLeaveComboBoxNameCommand;

        ///<summary>Команда когда курсор выходит с ComboBox (Название)</summary>
        public ICommand MouseLeaveComboBoxNameCommand => _mouseLeaveComboBoxNameCommand
            ??= new LambdaCommand(OnMouseLeaveComboBoxNameCommandExecuted);

        ///<summary>Логика выполнения - Команда когда курсор выходит с ComboBox (Название)</summary>
        private void OnMouseLeaveComboBoxNameCommandExecuted(object? p)
        {
            _namesView.Filter -= NamesView_Filter;
            CallFinalizer();
        }

        #endregion

        #region MouseLeaveComboBoxPublishCommand - Команда когда курсор выходит с ComboBox (Издательство)

        ///<summary>Команда когда курсор выходит с ComboBox (Издательство)</summary>
        private ICommand? _mouseLeaveComboBoxPublishCommand;

        ///<summary>Команда когда курсор выходит с ComboBox (Издательство)</summary>
        public ICommand MouseLeaveComboBoxPublishCommand => _mouseLeaveComboBoxPublishCommand
            ??= new LambdaCommand(OnMouseLeaveComboBoxPublishCommandExecuted);

        ///<summary>Логика выполнения - Команда когда курсор выходит с ComboBox (Издательство)</summary>
        private void OnMouseLeaveComboBoxPublishCommandExecuted(object? p)
        {
            _publishView.Filter -= PublishView_Filter;
            CallFinalizer();
        }

        #endregion

        #region MouseLeaveComboBoxYearPublishCommand - Команда когда курсор выходит с ComboBox (Год издания)

        ///<summary>Команда когда курсор выходит с ComboBox (Год издания)</summary>
        private ICommand? _mouseLeaveComboBoxYearPublishCommand;

        ///<summary>Команда когда курсор выходит с ComboBox (Год издания)</summary>
        public ICommand MouseLeaveComboBoxYearPublishCommand => _mouseLeaveComboBoxYearPublishCommand
            ??= new LambdaCommand(OnMouseLeaveComboBoxYearPublishCommandExecuted);

        ///<summary>Логика выполнения - Команда когда курсор выходит с ComboBox (Год издания)</summary>
        private void OnMouseLeaveComboBoxYearPublishCommandExecuted(object? p)
        {
            _yearPublishView.Filter -= YearPublishView_Filter;
            CallFinalizer();
        }

        #endregion

        #region MouseLeaveTextBoxBooksCommand - Команда когда курсор выходит с TextBox

        ///<summary>Команда когда курсор выходит с TextBox</summary>
        private ICommand? _mouseLeaveTextBoxBooksCommand;

        ///<summary>Команда когда курсор выходит с TextBox</summary>
        public ICommand MouseLeaveTextBoxBooksCommand => _mouseLeaveTextBoxBooksCommand
            ??= new LambdaCommand(OnMouseLeaveTextBoxBooksCommandExecuted);

        ///<summary>Логика выполнения - Команда когда курсор выходит с TextBox</summary>
        private void OnMouseLeaveTextBoxBooksCommandExecuted(object? p)
        {
            _booksView.Filter -= BooksView_Filter;
            CallFinalizer();
        }

        #endregion

        #endregion

        #endregion

        #region Events

        #region YearPublishView_Filter
        private void YearPublishView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not string yearPublish)
            {
                e.Accepted = false;
                return;
            }
            //var yearPublish_string = Convert.ToString(yearPublish);
            var filter_text = _selectedYearPublish;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || yearPublish.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                ;
        }
        #endregion

        #region PublishView_Filter
        private void PublishView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not string publish)
            {
                e.Accepted = false;
                return;
            }
            var filter_text = _selectedPublish;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || publish.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                ;
        }
        #endregion

        #region BBKView_Filter
        private void BBKView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not string bbk)
            {
                e.Accepted = false;
                return;
            }
            var filter_text = _selectedBBK;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || bbk.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                ;
        }
        #endregion

        #region StockView_Filter
        private void StockView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not string stock)
            {
                e.Accepted = false;
                return;
            }
            var filter_text = _selectedStock;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || stock.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                ;
        }
        #endregion

        #region NamesView_Filter
        private void NamesView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not string name)
            {
                e.Accepted = false;
                return;
            }
            var filter_text = _selectedName;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || name.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                ;
        }
        #endregion

        #region AuthorView_Filter
        private void AuthorsView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not string author)
            {
                e.Accepted = false;
                return;
            }
            var filter_text = _authorsFilterText;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || author.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                ;
        }
        #endregion

        #region BooksView_Filter
        private void BooksView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not Book book)
            {
                e.Accepted = false;
                return;
            }
            var filter_text = _booksFilterText;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                    || book.BBK.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                    || book.Author.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                    || book.Name.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                    || book.Publish.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                    || book.YearPublish.ToString().Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                    || book.CodeAuthor.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                    ;
        }
        #endregion

        #endregion

        public BooksUserControlViewModel()
        {
            Books = new(Enumerable.Range(0, 100000).Select(p => new Book
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
                Stock = Random.Shared.Next(0, 2) == 0 ? "Да" : "Нет"
            }));
            _booksView.Source = Books;
            OnPropertyChanged(nameof(BooksView));
            ((Command)ResetToZeroFindCommand).Executable = false;
        }

        private void CallFinalizer()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

    }
}
