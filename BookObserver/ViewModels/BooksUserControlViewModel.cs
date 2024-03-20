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
        public ObservableCollection<Book>? Books { get; }

        #region BooksView : ICollectionView - Вывод списка книг

        private readonly CollectionViewSource _booksView = new();
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

        #region StockFilterText : string? - Фильтр "в наличии"

        ///<summary>Фильтр "в наличии"</summary>
        private string? _stockFilterText;

        ///<summary>Фильтр "в наличии"</summary>
        public string? StockFilterText
        {
            get => _stockFilterText;
            set
            {
                if (!Set(ref _stockFilterText, value)) return;

                if (!((Command)FindBooksCommand).Executable)
                    ((Command)FindBooksCommand).Executable = true;
                _stockView.View.Refresh();
            }
        }

        #endregion

        #region BBKFilterText : string? - Фильтр ББК

        ///<summary>Фильтр ББК</summary>
        private string? _bbkFilterText;

        ///<summary>Фильтр ББК</summary>
        public string? BBKFilterText
        {
            get => _bbkFilterText;
            set
            {
                if (!Set(ref _bbkFilterText, value)) return;

                if (!((Command)FindBooksCommand).Executable)
                    ((Command)FindBooksCommand).Executable = true;
                _bbkView.View.Refresh();
            }
        }

        #endregion

        #region NameFilterText : string? - Фильтр названий

        ///<summary>Фильтр названий</summary>
        private string? _nameFilterText;

        ///<summary>Фильтр названий</summary>
        public string? NameFilterText
        {
            get => _nameFilterText;
            set
            {
                if (!Set(ref _nameFilterText, value)) return;

                if (!((Command)FindBooksCommand).Executable)
                    ((Command)FindBooksCommand).Executable = true;
                _namesView.View.Refresh();
            }
        }

        #endregion

        #region AuthorsFilterText : string? - Фильтр авторов

        ///<summary>Фильтр авторов</summary>
        private string? _authorsFilterText;

        ///<summary>Фильтр авторов</summary>
        public string? AuthorsFilterText
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

        #region PublishFilterText : string? - Фильтр издательств

        ///<summary>Фильтр издательств</summary>
        private string? _publishFilterText;

        ///<summary>Фильтр издательств</summary>
        public string? PublishFilterText
        {
            get => _publishFilterText;
            set
            {
                if (!Set(ref _publishFilterText, value)) return;

                if (!((Command)FindBooksCommand).Executable)
                    ((Command)FindBooksCommand).Executable = true;
                _publishView.View.Refresh();
            }
        }

        #endregion

        #region YearPublishFilterText : string? - Фильтр годов издания

        ///<summary>Фильтр годов издания</summary>
        private string? _yearPublishFilterText;

        ///<summary>Фильтр годов издания</summary>
        public string? YearPublishFilterText
        {
            get => _yearPublishFilterText;
            set
            {
                if (!Set(ref _yearPublishFilterText, value)) return;

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
            && (!string.IsNullOrWhiteSpace(_stockFilterText)
            || !string.IsNullOrWhiteSpace(_bbkFilterText)
            || !string.IsNullOrWhiteSpace(_authorsFilterText)
            || !string.IsNullOrWhiteSpace(_nameFilterText)
            || !string.IsNullOrWhiteSpace(_publishFilterText)
            || !string.IsNullOrWhiteSpace(_yearPublishFilterText)
            );

        ///<summary>Логика выполнения - Поиск книг</summary>
        private void OnFindBooksCommandExecuted(object? p)
        {
            IList<Book> result = (p as IList<Book>)!;
            if (!string.IsNullOrWhiteSpace(_stockFilterText)) result = result.Where(
                p => p.Stock.Contains(_stockFilterText, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_bbkFilterText)) result = result.Where(
                p => p.BBK.Contains(_bbkFilterText, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_authorsFilterText)) result = result.Where(
                p => p.Author.Contains(_authorsFilterText, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_nameFilterText)) result = result.Where(
                p => p.Name.Contains(_nameFilterText, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_publishFilterText)) result = result.Where(
                p => p.Publish.Contains(_publishFilterText, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_yearPublishFilterText)) result = result.Where(
                p => Convert.ToString(p.YearPublish)
                .Contains(_yearPublishFilterText, StringComparison.OrdinalIgnoreCase)).ToList();

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
            !string.IsNullOrWhiteSpace(_stockFilterText)
            || !string.IsNullOrWhiteSpace(_bbkFilterText)
            || !string.IsNullOrWhiteSpace(_authorsFilterText)
            || !string.IsNullOrWhiteSpace(_nameFilterText)
            || !string.IsNullOrWhiteSpace(_publishFilterText)
            || !string.IsNullOrWhiteSpace(_yearPublishFilterText);

        ///<summary>Логика выполнения - Команда для очистки фильтров</summary>
        private void OnClearFiltersCommandExecuted(object? p)
        {
            StockFilterText = null;
            BBKFilterText = null;
            AuthorsFilterText = null;
            NameFilterText = null;
            PublishFilterText = null;
            YearPublishFilterText = null;
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            _booksView.Source = Books;
            OnPropertyChanged(nameof(BooksView));
            SelectedBook = null;
            StockFilterText = null;
            BBKFilterText = null;
            AuthorsFilterText = null;
            NameFilterText = null;
            PublishFilterText = null;
            YearPublishFilterText = null;
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
            var value = _stockFilterText;
            _stockView.Source = Books?.Select(book => book.Stock).Distinct().Order().ToList();
            OnPropertyChanged(nameof(StockView));
            _stockView.Filter += StockView_Filter;
            StockFilterText = value;
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
            var value = _bbkFilterText;
            _bbkView.Source = Books?.Select(book => book.BBK).Distinct().Order().ToList();
            OnPropertyChanged(nameof(BBKView));
            _bbkView.Filter += BBKView_Filter;
            BBKFilterText = value;
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
            AuthorsFilterText = value;
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
            var value = _nameFilterText;
            _namesView.Source = Books?.Select(book => book.Name).Distinct().Order().ToList();
            OnPropertyChanged(nameof(NamesView));
            _namesView.Filter += NamesView_Filter;
            NameFilterText = value;
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
            var value = _publishFilterText;
            _publishView.Source = Books?.Select(book => book.Publish).Distinct().Order().ToList();
            OnPropertyChanged(nameof(PublishView));
            _publishView.Filter += PublishView_Filter;
            PublishFilterText = value;
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
            var value = _yearPublishFilterText;
            _yearPublishView.Source = Books?.Select(book => book.YearPublish).Distinct().Order().ToList();
            OnPropertyChanged(nameof(YearPublishView));
            _yearPublishView.Filter += YearPublishView_Filter;
            YearPublishFilterText = value;
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        #endregion

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
            var filter_text = _yearPublishFilterText;
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
            var filter_text = _publishFilterText;
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
            var filter_text = _bbkFilterText;
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
            var filter_text = _stockFilterText;
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
            var filter_text = _nameFilterText;
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
    }
}
