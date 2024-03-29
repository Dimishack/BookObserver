﻿using BookObserver.Infrastructure.Commands;
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
        /// <summary>Список книг</summary>
        public ObservableCollection<Reader> Readers { get; }

        #region FiltredReaders : ObservableCollection<Reader>? - Отфильтрованный список читателей

        ///<summary>Отфильтрованный список читателей</summary>
        private ObservableCollection<Reader>? _filtredReaders;

        ///<summary>Отфильтрованный список читателей</summary>
        private ObservableCollection<Reader>? FiltredReaders { get => _filtredReaders; set => Set(ref _filtredReaders, value); }

        #endregion

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

        #region MinDateGet : DateTime - Минимальный промежуток дат получения

        ///<summary>Минимальный промежуток дат получения</summary>
        private DateTime _minDateGet = DateTime.MinValue;

        ///<summary>Минимальный промежуток дат получения</summary>
        public DateTime MinDateGet { get => _minDateGet; set => Set(ref _minDateGet, value); }

        #endregion

        #region MaxDateGet : DateTime - Максимальный промежуток дат получения

        ///<summary>Максимальный промежуток дат получения</summary>
        private DateTime _maxDateGet = DateTime.MaxValue;

        ///<summary>Максимальный промежуток дат получения</summary>
        public DateTime MaxDateGet { get => _maxDateGet; set => Set(ref _maxDateGet, value); }

        #endregion

        #region MinDateSet : DateTime - Минимальный промежуток дат возварата

        ///<summary>Минимальный промежуток дат возварата</summary>
        private DateTime _minDateSet = DateTime.MinValue;

        ///<summary>Минимальный промежуток дат возварата</summary>
        public DateTime MinDateSet { get => _minDateSet; set => Set(ref _minDateSet, value); }

        #endregion

        #region MaxDateSet : DateTime - Максимальный промежуток дат возврата

        ///<summary>Максимальный промежуток дат возврата</summary>
        private DateTime _maxDateSet = DateTime.MaxValue;

        ///<summary>Максимальный промежуток дат возврата</summary>
        public DateTime MaxDateSet { get => _maxDateSet; set => Set(ref _maxDateSet, value); }

        #endregion

        #region ReadersView : ICollectionView - Вывод списка читателей

        private readonly CollectionViewSource _readersView = new();
        public ICollectionView ReadersView => _readersView.View;
        #endregion

        #region FiltredLastNames : ObservableCollection<string>? - Отфильтрованный список фамилий

        ///<summary>Список фамилий</summary>
        private ObservableCollection<string>? _lastNames { get; set; }

        ///<summary>Отфильтрованный список фамилий</summary>
        private ObservableCollection<string>? _filtredLastNames;

        ///<summary>Отфильтрованный список фамилий</summary>
        public ObservableCollection<string>? FiltredLastNames { get => _filtredLastNames; set => Set(ref _filtredLastNames, value); }

        #endregion

        #region FiltredFirstNames : ObservableCollection<string>? - Отфильтрованный список имен

        /// <summary>Список имен</summary>
        private ObservableCollection<string>? _firstNames;

        ///<summary>Отфильтрованный список имен</summary>
        private ObservableCollection<string>? _filtredFirstNames;

        ///<summary>Отфильтрованный список имен</summary>
        public ObservableCollection<string>? FiltredFirstNames { get => _filtredFirstNames; set => Set(ref _filtredFirstNames, value); }

        #endregion

        #region FitlredPatronymics : ObservableCollection<string>? - Отфильтрованный список отчеств

        /// <summary>Список отчеств</summary>
        private ObservableCollection<string>? _patronymics { get; set; }

        ///<summary>Отфильтрованный список отчеств</summary>
        private ObservableCollection<string>? _filtredPatronymics;

        ///<summary>Отфильтрованный список отчеств</summary>
        public ObservableCollection<string>? FiltredPatronymics { get => _filtredPatronymics; set => Set(ref _filtredPatronymics, value); }

        #endregion

        #region SelectedReader : Reader? - Выбранный читатель

        ///<summary>Выбранный читатель</summary>
        private Reader? _selectedReader;

        ///<summary>Выбранный читатель</summary>
        public Reader? SelectedReader { get => _selectedReader; set => Set(ref _selectedReader, value); }

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

                if (_lastNames is not null)
                {
                    _filtredLastNames = null;
                    ClearGarbage();
                    FiltredLastNames = new(_lastNames.Where(ln => ln.Contains(value, StringComparison.OrdinalIgnoreCase)));
                }

                ExecutableOnSearchCommandChangeOnTrue();
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

                if (_firstNames is not null)
                {
                    _filtredFirstNames = null;
                    ClearGarbage();
                    FiltredFirstNames = new(_firstNames.Where(fn => fn.Contains(value, StringComparison.OrdinalIgnoreCase)));
                }
                ExecutableOnSearchCommandChangeOnTrue();
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

                if (_patronymics is not null)
                {
                    _filtredPatronymics = null;
                    ClearGarbage();
                    FiltredPatronymics = new(_patronymics.Where(p => p.Contains(value, StringComparison.OrdinalIgnoreCase)));
                }
            }
        }

        #endregion

        #region SelectedGetDateFrom : DateTime? - Выбранный промежуток дат получения (от)

        ///<summary>Выбранный промежуток дат получения (от)</summary>
        private DateTime? _selectedGetDateFrom;

        ///<summary>Выбранный промежуток дат получения (от)</summary>
        public DateTime? SelectedGetDateFrom
        {
            get => _selectedGetDateFrom;
            set
            {
                if (!Set(ref _selectedGetDateFrom, value)) return;

                ExecutableOnSearchCommandChangeOnTrue();
            }
        }

        #endregion

        #region SelectedGetDateTo : DateTime? - Выбранный промежуток дат получения (до)

        ///<summary>Выбранный промежуток дат получения (до)</summary>
        private DateTime? _selectedGetDateTo;

        ///<summary>Выбранный промежуток дат получения (до)</summary>
        public DateTime? SelectedGetDateTo
        {
            get => _selectedGetDateTo;
            set
            {
                if (!Set(ref _selectedGetDateTo, value)) return;

                ExecutableOnSearchCommandChangeOnTrue();
            }
        }

        #endregion

        #region SelectedSetDateFrom : DateTime? - Выбранный промежуток дат возврата (от)

        ///<summary>Выбранный промежуток дат возврата (от)</summary>
        private DateTime? _selectedSetDateFrom;

        ///<summary>Выбранный промежуток дат возврата (от)</summary>
        public DateTime? SelectedSetDateFrom
        {
            get => _selectedSetDateFrom;
            set
            {
                if (!Set(ref _selectedSetDateFrom, value)) return;

                ExecutableOnSearchCommandChangeOnTrue();
            }
        }

        #endregion

        #region SelectedSetDateTo : DateTime? - Выбранный промежуток дат возврата (до)

        ///<summary>Выбранный промежуток дат возврата (до)</summary>
        private DateTime? _selectedSetDateTo;

        ///<summary>Выбранный промежуток дат возврата (до)</summary>
        public DateTime? SelectedSetDateTo
        {
            get => _selectedSetDateTo;
            set
            {
                if (!Set(ref _selectedSetDateTo, value)) return;

                ExecutableOnSearchCommandChangeOnTrue();
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
                ClearGarbage();
                _readersView.View.SortDescriptions.Add(Sorting[value]);
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
        private void OnDeleteReaderCommandExecuted(object? p)
        {
            FiltredReaders.Remove((p as Reader)!);
            Readers.Remove((p as Reader)!);
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
            || _selectedGetDateFrom is not null
            || _selectedGetDateTo is not null
            || _selectedSetDateFrom is not null
            || _selectedSetDateTo is not null
            ;

        ///<summary>Логика выполнения - Команда очистка полей для поиска</summary>
        private void OnClearFieldsForSearchCommandExecuted(object? p)
        {
            SelectedLastName = null;
            SelectedFirstName = null;
            SelectedPatronymic = null;
            SelectedGetDateFrom = null;
            SelectedGetDateTo = null;
            SelectedSetDateFrom = null;
            SelectedSetDateTo = null;
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
            || _selectedGetDateFrom is not null
            || _selectedGetDateTo is not null
            || _selectedSetDateFrom is not null
            || _selectedSetDateTo is not null
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
            result = result.Where(r => r.DateGet >= (_selectedGetDateFrom ?? _minDateGet)
                        && r.DateGet <= (_selectedGetDateTo ?? _maxDateGet)).ToList();
            result = result.Where(r => r.DateSet >= (_selectedSetDateFrom ?? _minDateSet)
                        && r.DateSet <= (_selectedSetDateTo ?? _maxDateSet)).ToList();

            FiltredReaders = null;
            _readersView.View.SortDescriptions.Clear();
            ClearGarbage();
            _readersView.Source = FiltredReaders = new(result);
            _readersView.View.SortDescriptions.Add(Sorting[_selectedSorting]);
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
            _readersView.Source = FiltredReaders = Readers;
            OnPropertyChanged(nameof(ReadersView));
            ((Command)ResetToZeroSearchCommand).Executable = false;
            ExecutableOnSearchCommandChangeOnTrue();
        }

        #endregion

        #region GotFocusComboBoxLastNamesCommand - Команда при получении фокуса (ComboBox фамилии)

        ///<summary>Команда при получении фокуса (ComboBox фамилии)</summary>
        private ICommand? _gotFocusComboBoxLastNamesCommand;

        ///<summary>Команда при получении фокуса (ComboBox фамилии)</summary>
        public ICommand GotFocusComboBoxLastNamesCommand => _gotFocusComboBoxLastNamesCommand
            ??= new LambdaCommand(OnGotFocusComboBoxLastNamesCommandExecuted, CanGotFocusComboBoxLastNamesCommandExecute);

        private bool CanGotFocusComboBoxLastNamesCommandExecute(object? p) =>
            p is not null && p is IList<Reader>;

        ///<summary>Логика выполнения - Команда при получении фокуса (ComboBox фамилии)</summary>
        private void OnGotFocusComboBoxLastNamesCommandExecuted(object? p) =>
            FiltredLastNames
            = _lastNames
            = new ObservableCollection<string>((p as IList<Reader>)!.Select(r => r.LastName).Distinct().Order()!);

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
        private void OnGotFocusComboBoxFirstNamesCommandExecuted(object? p) =>
            FiltredFirstNames
            = _firstNames
            = new((p as IList<Reader>)!.Select(r => r.FirstName).Distinct().Order()!);

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
        private void OnGotFocusComboBoxPatronymicsCommandExecuted(object? p) =>
            FiltredPatronymics
            = _patronymics
            = new((p as IList<Reader>)!.Select(r => r.Patronymic).Distinct().Order()!);

        #endregion

        #region GotFocusDatePickersGetDateCommand - Команда при получении фокуса (DatePickers дат получения)

        ///<summary>Команда при получении фокуса (DatePickers дат получения)</summary>
        private ICommand? _gotFocusDatePickersGetDateCommand;

        ///<summary>Команда при получении фокуса (DatePickers дат получения)</summary>
        public ICommand GotFocusDatePickersGetDateCommand => _gotFocusDatePickersGetDateCommand
            ??= new LambdaCommand(OnGotFocusDatePickersGetDateCommandExecuted, CanGotFocusDatePickersGetDateCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда при получении фокуса (DatePickers дат получения)</summary>
        private bool CanGotFocusDatePickersGetDateCommandExecute(object? p) =>
            p is not null && p is IList<Reader>;

        ///<summary>Логика выполнения - Команда при получении фокуса (DatePickers дат получения)</summary>
        private void OnGotFocusDatePickersGetDateCommandExecuted(object? p)
        {
            var listReader = (p as IList<Reader>).Select(r => r.DateGet);
            MinDateGet = listReader.Min();
            MaxDateGet = listReader.Max();
        }

        #endregion

        #region GotFocusDatePickersSetDateCommandCommand - Команда при получении фокуса (DatePickers дат возврата)

        ///<summary>Команда при получении фокуса (DatePickers дат возврата)</summary>
        private ICommand? _gotFocusDatePickersSetDateCommandCommand;

        ///<summary>Команда при получении фокуса (DatePickers дат возврата)</summary>
        public ICommand GotFocusDatePickersSetDateCommandCommand => _gotFocusDatePickersSetDateCommandCommand
            ??= new LambdaCommand(OnGotFocusDatePickersSetDateCommandCommandExecuted, CanGotFocusDatePickersSetDateCommandCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда при получении фокуса (DatePickers дат возврата)</summary>
        private bool CanGotFocusDatePickersSetDateCommandCommandExecute(object? p) =>
            p is not null && p is IList<Reader>;

        ///<summary>Логика выполнения - Команда при получении фокуса (DatePickers дат возврата)</summary>
        private void OnGotFocusDatePickersSetDateCommandCommandExecuted(object? p)
        {
            var listReader = (p as IList<Reader>).Select(r => r.DateSet);
            MinDateSet = listReader.Min();
            MaxDateSet = listReader.Max();
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
            _filtredLastNames = _lastNames = null;
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
            _filtredFirstNames = _firstNames = null;
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
            _filtredPatronymics = _patronymics = null;
            ClearGarbage();
        }

        #endregion

        #endregion

        public ReadersViewModel()
        {
            FiltredReaders = Readers = new ObservableCollection<Reader>(
                Enumerable.Range(1, 100000).Select(
                    p => new Reader
                    {
                        Id = p,
                        LastName = $"Фамилия {p}",
                        FirstName = $"Имя {p}",
                        Patronymic = $"Отчество {p}",
                        Telephone = $"Телефон {p}",
                        Address = $"Адрес {p}",
                        DateGet = DateTime.Today.AddDays(Random.Shared.Next(0, 30)),
                        DateSet = DateTime.Today.AddMonths(1).AddDays(Random.Shared.Next(0, 30)),
                    }));

            ((Command)ResetToZeroSearchCommand).Executable = false;
            _readersView.Source = _filtredReaders;
        }

        private void ExecutableOnSearchCommandChangeOnTrue()
        {
            if (!((Command)SearchCommand).Executable)
                ((Command)SearchCommand).Executable = true;
        }
    }
}
