using BookObserver.Infrastructure.Commands;
using BookObserver.Models;
using BookObserver.Models.Readers;
using BookObserver.Services.Interfaces;
using BookObserver.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace BookObserver.ViewModels
{
    internal class CreatorReaderViewModel : ViewModel
    {
        private readonly IUserDialog _userDialog;
        private readonly ReadersViewModel _readersVM;

        public ObservableCollection<Reader> Readers { get; }

        #region Title : string - Заголовок окна

        ///<summary>Заголовок окна</summary>
        private string _title = "Добавить читателя";

        ///<summary>Заголовок окна</summary>
        public string Title { get => _title; set => Set(ref _title, value); }

        #endregion

        private readonly CollectionWithFilter _lastNamesView = new();
        public ObservableCollection<string>? LastNamesView => _lastNamesView.CollectionView;

        #region SelectedLastName : string - Выбранная фамилия

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
            }
        }

        #endregion

        private readonly CollectionWithFilter _firstNamesView = new();
        public ObservableCollection<string>? FirstNamesView => _firstNamesView.CollectionView;

        #region SelectedFirstName : string - Выбранное имя

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
            }
        }

        #endregion

        private readonly CollectionWithFilter _addressesView = new();
        public ObservableCollection<string>? AdressesView => _addressesView.CollectionView;

        #region SelectedAddress : string - Выбранный адрес

        ///<summary>Выбранный адрес</summary>
        private string _selectedAddress = string.Empty;

        ///<summary>Выбранный адрес</summary>
        public string SelectedAddress
        {
            get => _selectedAddress;
            set
            {
                if (!Set(ref _selectedAddress, value)) return;

                _addressesView.RefreshFilter(value);
                OnPropertyChanged(nameof(AdressesView));
            }
        }

        #endregion

        #region SelectedPhoneNumber : string - Выбранный номер телефона

        ///<summary>Выбранный номер телефона</summary>
        private string _selectedPhoneNumber = string.Empty;

        ///<summary>Выбранный номер телефона</summary>
        public string SelectedPhoneNumber
        {
            get => _selectedPhoneNumber;
            set => Set(ref _selectedPhoneNumber, value);

        }

        #endregion

        #region SelectedHomePhoneNumber : string - Выбранный домашний номер телефона

        ///<summary>Выбранный домашний номер телефона</summary>
        private string _selectedHomePhoneNumber = string.Empty;

        ///<summary>Выбранный домашний номер телефона</summary>
        public string SelectedHomePhoneNumber
        {
            get => _selectedHomePhoneNumber;
            set => Set(ref _selectedHomePhoneNumber, value);

        }

        #endregion

        #region IsNotifyAddReader : bool - Разрешение уведомлять о добавлении читателя

        ///<summary>Разрешение уведомлять о добавлении читателя</summary>
        private bool _isNotifyAddReader;

        ///<summary>Разрешение уведомлять о добавлении читателя</summary>
        public bool IsNotifyAddReader { get => _isNotifyAddReader; set => Set(ref _isNotifyAddReader, value); }

        #endregion

        #region Commands...

        #region ClearFieldsCommand - Команда очистки полей

        ///<summary>Команда очистки полей</summary>
        private ICommand? _clearFieldsCommand;

        ///<summary>Команда очистки полей</summary>
        public ICommand ClearFieldsCommand => _clearFieldsCommand
            ??= new LambdaCommand(OnClearFieldsCommandExecuted, CanClearFieldsCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда очистки полей</summary>
        private bool CanClearFieldsCommandExecute(object? p) => 
            !string.IsNullOrWhiteSpace(_selectedLastName)
            || !string.IsNullOrWhiteSpace(_selectedFirstName)
            || !string.IsNullOrWhiteSpace(_selectedPatronymic)
            || !string.IsNullOrWhiteSpace(_selectedPhoneNumber)
            || !string.IsNullOrWhiteSpace(_selectedHomePhoneNumber)
            || !string.IsNullOrWhiteSpace(_selectedAddress)
            ;

        ///<summary>Логика выполнения - Команда очистки полей</summary>
        private void OnClearFieldsCommandExecuted(object? p)
        {
            SelectedLastName = string.Empty;
            SelectedFirstName = string.Empty;
            SelectedPatronymic = string.Empty;
            SelectedPhoneNumber = string.Empty;
            SelectedHomePhoneNumber = string.Empty;
            SelectedAddress = string.Empty;
        }

        #endregion

        #region CloseWindowCommand - Команда закрытия окна

        ///<summary>Команда закрытия окна</summary>
        private ICommand? _closeWindowCommand;

        ///<summary>Команда закрытия окна</summary>
        public ICommand CloseWindowCommand => _closeWindowCommand
            ??= new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecute);

        ///<summary>Проверка возможности выполнения - Команда закрытия окна</summary>
        private bool CanCloseWindowCommandExecute(object? p) => p is Window;

        ///<summary>Логика выполнения - Команда закрытия окна</summary>
        private void OnCloseWindowCommandExecuted(object? p) => (p as Window)!.Close();

        #endregion

        #region AddReaderCommand - Команда добавления читателя в основной список

        ///<summary>Команда добавления читателя в основной список</summary>
        private ICommand? _addReaderCommand;

        ///<summary>Команда добавления читателя в основной список</summary>
        public ICommand AddReaderCommand => _addReaderCommand
            ??= new LambdaCommand(OnAddReaderCommandExecuted);

        ///<summary>Логика выполнения - Команда добавления читателя в основной список</summary>
        private void OnAddReaderCommandExecuted(object? p)
        {
            Readers.Add(new Reader
            {
                Id = Readers.Count,
                LastName = _selectedLastName,
                FirstName = _selectedFirstName,
                Patronymic = _selectedPatronymic,
                Address = _selectedAddress,
                NumberPhone = _selectedPhoneNumber,
                HomeNumberPhone = _selectedHomePhoneNumber
            });
            _readersVM._readersView.View.Refresh();
            if (_isNotifyAddReader)
                _userDialog.ShowInformation("Читатель успешно добавлен в список", _title);

        }

        #endregion

        #endregion

        public CreatorReaderViewModel(ReadersViewModel readersVM, IUserDialog userDialog)
        {
            _userDialog = userDialog;
            _readersVM = readersVM;
            Readers = _readersVM.Readers;
            _lastNamesView.List = Readers.Select(r => r.LastName).Distinct().Order().ToList();
            _firstNamesView.List = Readers.Select(r => r.FirstName).Distinct().Order().ToList();
            _patronymicsView.List = Readers.Select(r => r.Patronymic).Distinct().Order().ToList();
            _addressesView.List = Readers.Select(r => r.Address).Distinct().Order().ToList();
        }

    }
}
