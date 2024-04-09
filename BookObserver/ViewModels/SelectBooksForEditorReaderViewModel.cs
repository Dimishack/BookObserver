using BookObserver.Infrastructure.Commands;
using BookObserver.Infrastructure.Commands.Base;
using BookObserver.Models;
using BookObserver.Models.Books;
using BookObserver.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    class SelectBooksForEditorReaderViewModel : ViewModel
    {
        private readonly BooksViewModel _booksVM;
        private readonly EditorReaderViewModel _editorReaderVM;

        public ObservableCollection<Book> BooksInExistence { get; }

        #region BooksInExistenceView : ObservableCollection<Book>? - Вывод списка книг в наличии

        ///<summary>Вывод списка книг в наличии</summary>
        private ObservableCollection<Book>? _booksInExistenceView;

        ///<summary>Вывод списка книг в наличии</summary>
        public ObservableCollection<Book>? BooksInExistenceView { get => _booksInExistenceView; set => Set(ref _booksInExistenceView, value); }

        #endregion

        #region SelectedBook : Book? - Выбранная книга

        ///<summary>Выбранная книга</summary>
        private Book? _selectedBook;

        ///<summary>Выбранная книга</summary>
        public Book? SelectedBook { get => _selectedBook; set => Set(ref _selectedBook, value); }

        #endregion

        private readonly CollectionWithFilter _bbksView = new();
        public ObservableCollection<string>? BBKsView => _bbksView.CollectionView;

        #region BBKTextForSearch : string - Текст ББК для поиска

        ///<summary>Текст ББК для поиска</summary>
        private string _bbkTextForSearch = string.Empty;

        ///<summary>Текст ББК для поиска</summary>
        public string BBKTextForSearch
        {
            get => _bbkTextForSearch;
            set
            {
                if (!Set(ref _bbkTextForSearch, value)) return;

                _bbksView.RefreshFilter(value);
                OnPropertyChanged(nameof(BBKsView));
                ((Command)SearchBooksCommand).Executable = true;
            }
        }

        #endregion

        private readonly CollectionWithFilter _authorsView = new();
        public ObservableCollection<string>? AuthorsView => _authorsView.CollectionView;

        #region AuthorTextForSearch : string - Текст Автор для поиска

        ///<summary>Текст Автор для поиска</summary>
        private string _authorsTextForSearch = string.Empty;

        ///<summary>Текст Автор для поиска</summary>
        public string AuthorTextForSearch
        {
            get => _authorsTextForSearch;
            set
            {
                if (!Set(ref _authorsTextForSearch, value)) return;

                _authorsView.RefreshFilter(value);
                OnPropertyChanged(nameof(AuthorsView));
                ((Command)SearchBooksCommand).Executable = true;
            }
        }

        #endregion

        private readonly CollectionWithFilter _namesView = new();
        public ObservableCollection<string>? NamesView => _namesView.CollectionView;

        #region NameTextForSearch : string - Текст Названия для поиска

        ///<summary>Текст Названия для поиска</summary>
        private string _nameTextForSearch = string.Empty;

        ///<summary>Текст Названия для поиска</summary>
        public string NameTextForSearch
        {
            get => _nameTextForSearch;
            set
            {
                if (!Set(ref _nameTextForSearch, value)) return;

                _namesView.RefreshFilter(value);
                OnPropertyChanged(nameof(NamesView));
                ((Command)SearchBooksCommand).Executable = true;
            }
        }

        #endregion

        private readonly CollectionWithFilter _publishesView = new();
        public ObservableCollection<string>? PublishesView => _publishesView.CollectionView;

        #region PublishTextForSearch : string - Текст Издательство для поиска

        ///<summary>Текст Издательство для поиска</summary>
        private string _publishTextForSearch = string.Empty;

        ///<summary>Текст Издательство для поиска</summary>
        public string PublishTextForSearch
        {
            get => _publishTextForSearch;
            set
            {
                if (!Set(ref _publishTextForSearch, value)) return;

                _publishesView.RefreshFilter(value);
                OnPropertyChanged(nameof(PublishesView));
                ((Command)SearchBooksCommand).Executable = true;
            }
        }

        #endregion

        #region Commands

        #region ClearFieldsCommand - Команда - очистки полей для поиска

        ///<summary>Команда - очистки полей для поиска</summary>
        private ICommand? _clearFieldsCommand;

        ///<summary>Команда - очистки полей для поиска</summary>
        public ICommand ClearFieldsCommand => _clearFieldsCommand
            ??= new LambdaCommand(OnClearFieldsCommandExecuted, CanClearFieldsCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда - очистки полей для поиска</summary>
        private bool CanClearFieldsCommandExecute(object? p) =>
            !string.IsNullOrWhiteSpace(_bbkTextForSearch)
            || !string.IsNullOrWhiteSpace(_authorsTextForSearch)
            || !string.IsNullOrWhiteSpace(_nameTextForSearch)
            || !string.IsNullOrWhiteSpace(_publishTextForSearch)
            ;

        ///<summary>Логика выполнения - Команда - очистки полей для поиска</summary>
        private void OnClearFieldsCommandExecuted(object? p)
        {
            BBKTextForSearch = string.Empty;
            AuthorTextForSearch = string.Empty;
            NameTextForSearch = string.Empty;
            PublishTextForSearch = string.Empty;
        }

        #endregion

        #region SearchBooksCommand - Команда - поиск книг

        ///<summary>Команда - поиск книг</summary>
        private ICommand? _searchBooksCommand;

        ///<summary>Команда - поиск книг</summary>
        public ICommand SearchBooksCommand => _searchBooksCommand
            ??= new LambdaCommand(OnSearchBooksCommandExecuted, CanSearchBooksCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда - поиск книг</summary>
        private bool CanSearchBooksCommandExecute(object? p) =>
            p is not null
            && p is IList<Book>
            && (!string.IsNullOrWhiteSpace(_bbkTextForSearch)
            || !string.IsNullOrWhiteSpace(_authorsTextForSearch)
            || !string.IsNullOrWhiteSpace(_nameTextForSearch)
            || !string.IsNullOrWhiteSpace(_publishTextForSearch)
            )
            ;

        ///<summary>Логика выполнения - Команда - поиск книг</summary>
        private void OnSearchBooksCommandExecuted(object? p)
        {
            IList<Book> result = (p as IList<Book>)!;
            if (!string.IsNullOrWhiteSpace(_bbkTextForSearch)) result = result.Where(
                p => p.BBK.Contains(_bbkTextForSearch, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_authorsTextForSearch)) result = result.Where(
                p => p.Author.Contains(_authorsTextForSearch, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_nameTextForSearch)) result = result.Where(
                p => p.Name.Contains(_nameTextForSearch, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(_publishTextForSearch)) result = result.Where(
                p => p.Publish.Contains(_publishTextForSearch, StringComparison.OrdinalIgnoreCase)).ToList();
            BooksInExistenceView = null;
            ClearGarbage();
            BooksInExistenceView = new(result);
            ((Command)ResetToZeroSearchCommand).Executable = true;
            ((Command)SearchBooksCommand).Executable = false;
        }

        #endregion

        #region ResetToZeroSearchCommand - Команда обнуления

        ///<summary>Команда обнуления</summary>
        private ICommand? _resetToZeroSearchCommand;

        ///<summary>Команда обнуления</summary>
        public ICommand ResetToZeroSearchCommand => _resetToZeroSearchCommand
            ??= new LambdaCommand(OnResetToZeroSearchCommandExecuted, CanResetToZeroSearchCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда обнуления</summary>
        private bool CanResetToZeroSearchCommandExecute(object? p) => true;

        ///<summary>Логика выполнения - Команда обнуления</summary>
        private void OnResetToZeroSearchCommandExecuted(object? p)
        {
            BooksInExistenceView = BooksInExistence;
            ((Command)ResetToZeroSearchCommand).Executable = false;
            ((Command)SearchBooksCommand).Executable = true;
        }

        #endregion

        #region SelectBookCommand - Команда выбрать книгу

        ///<summary>Команда выбрать книгу</summary>
        private ICommand? _selectBookCommand;

        ///<summary>Команда выбрать книгу</summary>
        public ICommand SelectBookCommand => _selectBookCommand
            ??= new LambdaCommand(OnSelectBookCommandExecuted, CanSelectBookCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда выбрать книгу</summary>
        private bool CanSelectBookCommandExecute(object? p) =>
            p is Window
            && _selectedBook is not null
            ;

        ///<summary>Логика выполнения - Команда выбрать книгу</summary>
        private void OnSelectBookCommandExecuted(object? p)
        {
            var window = (Window)p!;
            _editorReaderVM.IndexesBooks.Add(_selectedBook!.Id - 1);
            window.DialogResult = true;
            window.Close();
        }

        #endregion

        #region CancelCommand - Команда отмены

        ///<summary>Команда отмены</summary>
        private ICommand? _cancelCommand;

        ///<summary>Команда отмены</summary>
        public ICommand CancelCommand => _cancelCommand
            ??= new LambdaCommand(OnCancelCommandExecuted, CanCancelCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда отмены</summary>
        private bool CanCancelCommandExecute(object? p) => p is Window;

        ///<summary>Логика выполнения - Команда отмены</summary>
        private void OnCancelCommandExecuted(object? p) => (p as Window)!.Close();

        #endregion

        #endregion

        public SelectBooksForEditorReaderViewModel(EditorReaderViewModel editorReaderVM, BooksViewModel booksVM)
        {
            ((Command)ResetToZeroSearchCommand).Executable = false;

            _editorReaderVM = editorReaderVM;
            _booksVM = booksVM;
            BooksInExistence = new(_booksVM.Books.Where(b => b.Existence == "Да"));
            foreach (var indexBook in _editorReaderVM.IndexesBooks.OrderDescending())
                BooksInExistence.Remove(_booksVM.Books[indexBook]);
            BooksInExistenceView = BooksInExistence;
            _bbksView.List = BooksInExistence.Select(b => b.BBK).Distinct().Order().ToList();
            _authorsView.List = BooksInExistence.Select(b => b.Author).Distinct().Order().ToList();
            _namesView.List = BooksInExistence.Select(b => b.Name).Distinct().Order().ToList();
            _publishesView.List = BooksInExistence.Select(b => b.Publish).Distinct().Order().ToList();
        }
    }
}
