using BookObserver.Infrastructure.Commands;
using BookObserver.Infrastructure.Commands.Base;
using BookObserver.Models;
using BookObserver.Models.Readers;
using BookObserver.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    class SelectReaderForEditorBookViewModel : ViewModel
    {
        private readonly EditorBookViewModel _editorBookVM;
        private readonly ReadersViewModel _readersVM;

        /// <summary>Список книг</summary>
        public ObservableCollection<Reader> Readers { get; }

        #region FiltredReaders : ObservableCollection<Reader>? - Отфильтрованный список читателей

        ///<summary>Отфильтрованный список читателей</summary>
        private ObservableCollection<Reader>? _filtredReaders;

        ///<summary>Отфильтрованный список читателей</summary>
        public ObservableCollection<Reader>? FiltredReaders { get => _filtredReaders; set => Set(ref _filtredReaders, value); }

        #endregion

        #region SelectedReader : Reader? - Выбранный читатель

        ///<summary>Выбранный читатель</summary>
        private Reader? _selectedReader;

        ///<summary>Выбранный читатель</summary>
        public Reader? SelectedReader { get => _selectedReader; set => Set(ref _selectedReader, value); }

        #endregion

        private readonly CollectionWithFilter _lastNamesView = new();
        public ObservableCollection<string>? LastNamesView => _lastNamesView.CollectionView;

        #region SelectedLastName : string? - Выбранная фамилия для поиска

        ///<summary>Выбранная фамилия для поиска</summary>
        private string _selectedLastName = string.Empty;

        ///<summary>Выбранная фамилия для поиска</summary>
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

        #region SelectedFirstName : string - Выбранное имя для поиска

        ///<summary>Выбранное имя для поиска</summary>
        private string _selectedFirstName = string.Empty;

        ///<summary>Выбранное имя для поиска</summary>
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

        #region SelectedPatronymic : string - Выбранное отчество для поиска

        ///<summary>Выбранное отчество для поиска</summary>
        private string _selectedPatronymic = string.Empty;

        ///<summary>Выбранное отчество для поиска</summary>
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
            ClearGarbage();
            FiltredReaders = new(result);
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
            FiltredReaders = Readers;
            ((Command)ResetToZeroSearchCommand).Executable = false;
            ((Command)SearchCommand).Executable = true;
        }

        #endregion

        #region SelectReaderCommand - Команда выбрать читателя

        ///<summary>Команда выбрать читателя</summary>
        private ICommand? _selectReaderCommand;

        ///<summary>Команда выбрать читателя</summary>
        public ICommand SelectReaderCommand => _selectReaderCommand
            ??= new LambdaCommand(OnSelectReaderCommandExecuted, CanSelectReaderCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда выбрать читателя</summary>
        private bool CanSelectReaderCommandExecute(object? p) => 
            p is Window
            && SelectedReader is not null;

        ///<summary>Логика выполнения - Команда выбрать читателя</summary>
        private void OnSelectReaderCommandExecuted(object? p)
        {
            _editorBookVM.IndexReader = Readers.IndexOf(SelectedReader!);
            _editorBookVM.FullNameReader = SelectedReader!.FullName;
            (p as Window)!.Close();
        }

        #endregion

        #endregion

        public SelectReaderForEditorBookViewModel(EditorBookViewModel editorBookVM, ReadersViewModel readersVM)
        {
            _editorBookVM = editorBookVM;
            _readersVM = readersVM;
            FiltredReaders = Readers = _readersVM.Readers;
            _lastNamesView.List = Readers.Select(r => r.LastName).Distinct().Order().ToList()!;
            _firstNamesView.List = Readers.Select(r => r.FirstName).Distinct().Order().ToList()!;
            _patronymicsView.List = Readers.Select(r => r.Patronymic).Distinct().Order().ToList()!;


            ((Command)ResetToZeroSearchCommand).Executable = false;
        }
    }
}
