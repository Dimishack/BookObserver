using BookObserver.Infrastructure.Commands;
using BookObserver.Infrastructure.Commands.Base;
using BookObserver.Models.Readers;
using BookObserver.ViewModels.Base;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    internal class ReadersViewModel : ViewModel
    {
        public ObservableCollection<Reader> Readers { get; }

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
            {"Дата получения (по возрастанию)", new SortDescription("DateGet", ListSortDirection.Ascending)},
            {"Дата получения (по убыванию)", new SortDescription("DateGet", ListSortDirection.Descending)},
            {"Дата возврата (по возрастанию)", new SortDescription("DateSet", ListSortDirection.Ascending)},
            {"Дата возврата (по убыванию)", new SortDescription("DateSet", ListSortDirection.Descending)},
        };

        #region ReadersView : ICollectionView - Вывод списка читателей

        private readonly CollectionViewSource _readersView = new();
        public ICollectionView ReadersView => _readersView.View;
        #endregion

        #region LastNamesView : ICollectionView - Вывод списка фамилий

        private readonly CollectionViewSource _lastNamesView = new();
        public ICollectionView LastNamesView => _lastNamesView.View;

        #endregion

        #region FirstNamesView : ICollectionView - Вывод списка имен
        private readonly CollectionViewSource _firstNamesView = new();
        public ICollectionView FirstNamesView => _firstNamesView.View;
        #endregion

        #region PatronymicsView : ICollectionView - Вывод списка отчеств
        private readonly CollectionViewSource _patronymicsView = new();
        public ICollectionView PatronymicsView => _patronymicsView.View; 
        #endregion

        #region ReadersFilterText : string? - фильтрация списка читателей

        /////<summary>фильтрация списка читателей</summary>
        //private string? _readersFilterText;

        /////<summary>фильтрация списка читателей</summary>
        //public string? ReadersFilterText
        //{
        //    get => _readersFilterText;
        //    set
        //    {
        //        if (!Set(ref _readersFilterText, value)) return;

        //        _readersView.View.Refresh();
        //    }
        //}

        #endregion

        #region SelectedLastName : string? - Выбранная фамилия

        ///<summary>Выбранная фамилия</summary>
        private string? _selectedLastName;

        ///<summary>Выбранная фамилия</summary>
        public string? SelectedLastName
        {
            get => _selectedLastName;
            set
            {
                if (!Set(ref _selectedLastName, value)) return;

                _lastNamesView.View.Refresh();
                ExecutableOnSearchCommandChangedOnTrue();
            }
        }

        #endregion

        #region SelectedFirstName : string? - Выбранное имя

        ///<summary>Выбранное имя</summary>
        private string? _selectedFirstName;

        ///<summary>Выбранное имя</summary>
        public string? SelectedFirstName
        {
            get => _selectedFirstName;
            set
            {
                if (!Set(ref _selectedFirstName, value)) return;

                _firstNamesView.View.Refresh();
                ExecutableOnSearchCommandChangedOnTrue();
            }
        }

        #endregion

        #region SelectedPatronymic : string? - Выбранное отчество

        ///<summary>Выбранное отчество</summary>
        private string? _selectedPatronymic;

        ///<summary>Выбранное отчество</summary>
        public string? SelectedPatronymic
        {
            get => _selectedPatronymic;
            set
            {
                if (!Set(ref _selectedPatronymic, value)) return;

                _patronymicsView.View.Refresh();
            }
        }

        #endregion


        #region SelectedSorting : string? - Выбранная сортировка списка читателей

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
                _readersView.View.SortDescriptions.Add(Sorting[value]);
                ClearGarbage();
            }
        }

        #endregion

        #region Commands

        #region DeleteReaderCommand - Команда удаления читателя

        ///<summary>Команда удаления читателя</summary>
        private ICommand? _deleteReaderCommand;

        ///<summary>Команда удаления читателя</summary>
        public ICommand DeleteReaderCommand => _deleteReaderCommand
            ??= new LambdaCommand(OnDeleteReaderCommandExecuted, CanDeleteReaderCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда удаления читателя</summary>
        private bool CanDeleteReaderCommandExecute(object? p) => p is Reader;

        ///<summary>Логика выполнения - Команда удаления читателя</summary>
        private void OnDeleteReaderCommandExecuted(object? p) => Readers.Remove((p as Reader)!);

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
            SelectedLastName = null;
            SelectedFirstName = null;
            SelectedPatronymic = null;
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
                result = result.Where(r => r.LastName.Contains(_selectedLastName)).ToList();
            if (!string.IsNullOrWhiteSpace(_selectedFirstName))
                result = result.Where(r => r.FirstName.Contains(_selectedFirstName)).ToList();
            if (!string.IsNullOrWhiteSpace(_selectedPatronymic))
                result = result.Where(r => r.Patronymic.Contains(_selectedPatronymic)).ToList();

            _readersView.Source = result;
            _readersView.View.SortDescriptions.Clear();
            _readersView.View.SortDescriptions.Add(Sorting[_selectedSorting]);
            ClearGarbage();
            OnPropertyChanged(nameof(ReadersView));
            ((Command)SearchCommand).Executable = false;
            if (!((Command)ResetToZeroSearchCommand).Executable)
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
            _readersView.Source = Readers;
            OnPropertyChanged(nameof(ReadersView));
            ((Command)ResetToZeroSearchCommand).Executable = false;
            ExecutableOnSearchCommandChangedOnTrue();
        }

        #endregion

        #region GotFocusComboBoxLastNamesCommand - Команда при получении фокуса (ComboBox фамилии)

        ///<summary>Команда при получении фокуса (ComboBox фамилии)</summary>
        private ICommand? _gotFocusComboBoxLastNamesCommand;

        ///<summary>Команда при получении фокуса (ComboBox фамилии)</summary>
        public ICommand GotFocusComboBoxLastNamesCommand => _gotFocusComboBoxLastNamesCommand
            ??= new LambdaCommand(OnGotFocusComboBoxLastNamesCommandExecuted);

        ///<summary>Логика выполнения - Команда при получении фокуса (ComboBox фамилии)</summary>
        private void OnGotFocusComboBoxLastNamesCommandExecuted(object? p)
        {
            var value = _selectedLastName;
            _lastNamesView.Source = Readers.Select(p => p.LastName).Distinct().Order().ToList();
            OnPropertyChanged(nameof(LastNamesView));
            SelectedLastName = value;
            _lastNamesView.Filter += LastNamesView_Filter;
        }

        #endregion

        #region GotFocusComboBoxFirstNamesCommand - Команда при получении фокуса (ComboBox имена)

        ///<summary>Команда при получении фокуса (ComboBox имена)</summary>
        private ICommand? _gotFocusComboBoxFirstNamesCommand;

        ///<summary>Команда при получении фокуса (ComboBox имена)</summary>
        public ICommand GotFocusComboBoxFirstNamesCommand => _gotFocusComboBoxFirstNamesCommand
            ??= new LambdaCommand(OnGotFocusComboBoxFirstNamesCommandExecuted);

        ///<summary>Логика выполнения - Команда при получении фокуса (ComboBox имена)</summary>
        private void OnGotFocusComboBoxFirstNamesCommandExecuted(object? p)
        {
            var value = _selectedFirstName;
            _firstNamesView.Source = Readers.Select(p => p.FirstName).Distinct().Order().ToList();
            OnPropertyChanged(nameof(FirstNamesView));
            SelectedFirstName = value;
            _firstNamesView.Filter += FirstNamesView_Filter;
        }

        #endregion

        #region GotFocusComboBoxPatronymicsCommand - Команда при получении фокуса (ComboBox отчества)

        ///<summary>Команда при получении фокуса (ComboBox отчества)</summary>
        private ICommand? _gotFocusComboBoxPatronymicsCommand;

        ///<summary>Команда при получении фокуса (ComboBox отчества)</summary>
        public ICommand GotFocusComboBoxPatronymicsCommand => _gotFocusComboBoxPatronymicsCommand
            ??= new LambdaCommand(OnGotFocusComboBoxPatronymicsCommandExecuted);

        ///<summary>Логика выполнения - Команда при получении фокуса (ComboBox отчества)</summary>
        private void OnGotFocusComboBoxPatronymicsCommandExecuted(object? p)
        {
            var value = _selectedPatronymic;
            _patronymicsView.Source = (p as IList<Reader>)?.Select(r => r.Patronymic).Distinct().Order().ToList();
            OnPropertyChanged(nameof(PatronymicsView));
            SelectedPatronymic = value;
            _patronymicsView.Filter += PatronymicsView_Filter;
        }

        #endregion

        #region LostFocusComboBoxLastNamesCommand - Команда при потере фокуса (ComboBox фамилии)

        ///<summary>Команда при потере фокуса (ComboBox фамилии)</summary>
        private ICommand? _lostFocusComboBoxLastNamesCommand;

        ///<summary>Команда при потере фокуса (ComboBox фамилии)</summary>
        public ICommand LostFocusComboBoxLastNamesCommand => _lostFocusComboBoxLastNamesCommand
            ??= new LambdaCommand(OnLostFocusComboBoxLastNamesCommandExecuted);

        ///<summary>Логика выполнения - Команда при потере фокуса (ComboBox фамилии)</summary>
        private void OnLostFocusComboBoxLastNamesCommandExecuted(object? p)
        {
            _lastNamesView.Filter -= LastNamesView_Filter;
            ClearGarbage();
        }

        #endregion

        #region LostFocusComboBoxFirstNamesCommand - Команда при потере фокуса (ComboBox имена)

        ///<summary>Команда при потере фокуса (ComboBox имена)</summary>
        private ICommand? _lostFocusComboBoxFirstNamesCommand;

        ///<summary>Команда при потере фокуса (ComboBox имена)</summary>
        public ICommand LostFocusComboBoxFirstNamesCommand => _lostFocusComboBoxFirstNamesCommand
            ??= new LambdaCommand(OnLostFocusComboBoxFirstNamesCommandExecuted);

        ///<summary>Логика выполнения - Команда при потере фокуса (ComboBox имена)</summary>
        private void OnLostFocusComboBoxFirstNamesCommandExecuted(object? p)
        {
            _firstNamesView.Filter -= FirstNamesView_Filter;
            ClearGarbage();
        }

        #endregion

        #region LostFocusComboBoxPatronymicsCommand - Команда при потере фокуса (ComboBox отчества)

        ///<summary>Команда при потере фокуса (ComboBox отчества)</summary>
        private ICommand? _lostFocusComboBoxPatronymicCommand;

        ///<summary>Команда при потере фокуса (ComboBox отчества)</summary>
        public ICommand LostFocusComboBoxPatronymicsCommand => _lostFocusComboBoxPatronymicCommand
            ??= new LambdaCommand(OnLostFocusComboBoxPatronymicsCommandExecuted);

        ///<summary>Логика выполнения - Команда при потере фокуса (ComboBox отчества)</summary>
        private void OnLostFocusComboBoxPatronymicsCommandExecuted(object? p)
        {
            _patronymicsView.Filter -= PatronymicsView_Filter;
            ClearGarbage();
        }

        #endregion

        #endregion

        public ReadersViewModel()
        {
            Readers = new ObservableCollection<Reader>(
                Enumerable.Range(1, 100000).Select(
                    p => new Reader
                    {
                        Id = p,
                        LastName = $"Фамилия {p}",
                        FirstName = $"Имя {p}",
                        Patronymic = $"Отчество {p}",
                        Telephone = $"Телефон {p}",
                        Address = $"Адрес {p}"
                    }));
            ((Command)ResetToZeroSearchCommand).Executable = false;
            _readersView.Source = Readers;
            //_readersView.Filter += ReadersView_Filter;
        }

        private void ReadersView_Filter(object sender, FilterEventArgs e)
        {
            //if (e.Item is not Reader reader)
            //{
            //    e.Accepted = false;
            //    return;
            //}
            //var filter_text = _readersFilterText;

            //e.Accepted = string.IsNullOrWhiteSpace(filter_text)
            //    || reader.LastName.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
            //    || reader.FirstName.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
            //    || reader.Patronymic.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
            //    || reader.Telephone.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
            //    || reader.Address.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
            //    || reader.DateGet.ToString("dd.MM.yyyy").Contains(filter_text, StringComparison.OrdinalIgnoreCase)
            //    || reader.DateSet.ToString("dd.MM.yyyy").Contains(filter_text, StringComparison.OrdinalIgnoreCase)
            //    ;
        }

        private void LastNamesView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not string lastName)
            {
                e.Accepted = false;
                return;
            }

            var filter_text = _selectedLastName;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || lastName.Contains(filter_text)
                ;
        }

        private void FirstNamesView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not string firstName)
            {
                e.Accepted = false;
                return;
            }

            var filter_text = _selectedFirstName;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || firstName.Contains(filter_text)
                ;
        }

        private void PatronymicsView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not string patronymic)
            {
                e.Accepted = false;
                return;
            }

            var filter_text = _selectedPatronymic;
            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || patronymic.Contains(filter_text);
        }


        private void ExecutableOnSearchCommandChangedOnTrue()
        {
            if (!((Command)SearchCommand).Executable)
                ((Command)SearchCommand).Executable = true;
        }
    }
}
