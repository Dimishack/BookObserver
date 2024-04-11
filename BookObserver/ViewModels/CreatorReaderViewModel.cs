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

        #region LastName : string - Фамилия

        ///<summary>Фамилия</summary>
        private string _lastName = string.Empty;

        ///<summary>Фамилия</summary>
        public string LastName
        {
            get => _lastName;
            set
            {
                if (!Set(ref _lastName, value)) return;

                _lastNamesView.RefreshFilter(value);
                OnPropertyChanged(nameof(LastNamesView));
            }
        }

        #endregion

        private readonly CollectionWithFilter _firstNamesView = new();
        public ObservableCollection<string>? FirstNamesView => _firstNamesView.CollectionView;

        #region FirstName : string - Имя

        ///<summary>Имя</summary>
        private string _firstName = string.Empty;

        ///<summary>Имя</summary>
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (!Set(ref _firstName, value)) return;

                _firstNamesView.RefreshFilter(value);
                OnPropertyChanged(nameof(FirstNamesView));
            }
        }

        #endregion

        private readonly CollectionWithFilter _patronymicsView = new();
        public ObservableCollection<string>? PatronymicsView => _patronymicsView.CollectionView;

        #region Patronymic : string - Отчество

        ///<summary>Отчество</summary>
        private string _patronymic = string.Empty;

        ///<summary>Отчество</summary>
        public string Patronymic
        {
            get => _patronymic;
            set
            {
                if (!Set(ref _patronymic, value)) return;

                _patronymicsView.RefreshFilter(value);
                OnPropertyChanged(nameof(PatronymicsView));
            }
        }

        #endregion

        private readonly CollectionWithFilter _addressesView = new();
        public ObservableCollection<string>? AddressesView => _addressesView.CollectionView;

        #region Address : string - Адрес

        ///<summary>Адрес</summary>
        private string _address = string.Empty;

        ///<summary>Адрес</summary>
        public string Address
        {
            get => _address;
            set
            {
                if (!Set(ref _address, value)) return;

                _addressesView.RefreshFilter(value);
                OnPropertyChanged(nameof(AddressesView));
            }
        }

        #endregion

        #region PhoneNumber : string - Номер телефона

        ///<summary>Номер телефона</summary>
        private string _phoneNumber = string.Empty;

        ///<summary>Номер телефона</summary>
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => Set(ref _phoneNumber, value);

        }

        #endregion

        #region HomePhoneNumber : string - Домашний номер телефона

        ///<summary>Домашний номер телефона</summary>
        private string _homePhoneNumber = string.Empty;

        ///<summary>Домашний номер телефона</summary>
        public string HomePhoneNumber
        {
            get => _homePhoneNumber;
            set => Set(ref _homePhoneNumber, value);

        }

        #endregion

        #region IsNotifyAddReader : bool - Разрешение уведомлять о добавлении читателя

        ///<summary>Разрешение уведомлять о добавлении читателя</summary>
        private bool _isNotifyAddReader = true;

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
            !string.IsNullOrWhiteSpace(_lastName)
            || !string.IsNullOrWhiteSpace(_firstName)
            || !string.IsNullOrWhiteSpace(_patronymic)
            || !string.IsNullOrWhiteSpace(_phoneNumber)
            || !string.IsNullOrWhiteSpace(_homePhoneNumber)
            || !string.IsNullOrWhiteSpace(_address)
            ;

        ///<summary>Логика выполнения - Команда очистки полей</summary>
        private void OnClearFieldsCommandExecuted(object? p)
        {
            LastName = string.Empty;
            FirstName = string.Empty;
            Patronymic = string.Empty;
            PhoneNumber = string.Empty;
            HomePhoneNumber = string.Empty;
            Address = string.Empty;
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
            if ((string.IsNullOrWhiteSpace(_lastName)
                || string.IsNullOrWhiteSpace(_firstName)
                || string.IsNullOrWhiteSpace(_patronymic)
                || string.IsNullOrWhiteSpace(_phoneNumber)
                || string.IsNullOrWhiteSpace(_homePhoneNumber)
                || string.IsNullOrWhiteSpace(_address))
                && !_userDialog.ShowWarning("Обнаружены пустые поля.\nВсе равно добавить читателя в список?", _title)) return;

            Readers.Add(new Reader
            {
                Id = Readers.Count,
                LastName = _lastName,
                FirstName = _firstName,
                Patronymic = _patronymic,
                Address = _address,
                PhoneNumber = _phoneNumber,
                HomePhoneNumber = _homePhoneNumber
            });
            _readersVM._readersView.View.Refresh();
            ClearFieldsCommand.Execute(null);
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
