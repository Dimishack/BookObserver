﻿using BookObserver.Infrastructure.Commands;
using BookObserver.Infrastructure.Commands.Base;
using BookObserver.Models;
using BookObserver.Models.Readers;
using BookObserver.ViewModels.Base;
using BookObserver.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    internal class ReadersViewModel : ViewModel
    {
        private CreatorReaderWindow? _creatorReaderWindow;
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

                ((Command)SearchCommand).Executable = true;
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

                ((Command)SearchCommand).Executable = true;
            }
        }

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

                ((Command)SearchCommand).Executable = true;
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
            SelectedLastName = string.Empty;
            SelectedFirstName = string.Empty;
            SelectedPatronymic = string.Empty;
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
            var listReader = (p as IList<Reader>)!.Select(r => r.DateGet);
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
            var listReader = (p as IList<Reader>)!.Select(r => r.DateSet);
            MinDateSet = listReader.Min();
            MaxDateSet = listReader.Max();
        }

        #endregion

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
                        NumberPhone = $"Телефон {p}",
                        //HomeNumberPhone = $"Домашний {p}",
                        Address = $"Адрес {p}",
                        DateGet = DateTime.Today.AddDays(Random.Shared.Next(0, 30)),
                        DateSet = DateTime.Today.AddMonths(1).AddDays(Random.Shared.Next(0, 30)),
                    }));

            ((Command)ResetToZeroSearchCommand).Executable = false;
            _readersView.Source = _filtredReaders;
        }
    }
}
