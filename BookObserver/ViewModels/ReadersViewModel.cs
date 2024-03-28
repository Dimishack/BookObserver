using BookObserver.Infrastructure.Commands;
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

        #region ReadersView : ICollectionView - Вывод списка читателей
        private readonly CollectionViewSource _readersView = new();
        public ICollectionView ReadersView => _readersView.View;
        #endregion

        #region ReadersFilterText : string? - фильтрация списка читателей

        ///<summary>фильтрация списка читателей</summary>
        private string? _readersFilterText;

        ///<summary>фильтрация списка читателей</summary>
        public string? ReadersFilterText
        {
            get => _readersFilterText;
            set
            {
                if (!Set(ref _readersFilterText, value)) return;

                _readersView.View.Refresh();
            }
        }

        #endregion

        #region Commands

        #endregion

        public ReadersViewModel()
        {
            Readers = new ObservableCollection<Reader>(
                Enumerable.Range(1, 10000).Select(
                    p => new Reader
                    {
                        Id = p,
                        LastName = $"Фамилия {p}",
                        FirstName = $"Имя {p}",
                        Patronymic = $"Отчество {p}",
                        Telephone = $"Телефон {p}",
                        Address = $"Адрес {p}"
                    }));
            _readersView.Source = Readers;
            _readersView.Filter += ReadersView_Filter;
        }
        private void ReadersView_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is not Reader reader)
            {
                e.Accepted = false;
                return;
            }
            var filter_text = _readersFilterText;

            e.Accepted = string.IsNullOrWhiteSpace(filter_text)
                || reader.LastName.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                || reader.FirstName.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                || reader.Patronymic.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                || reader.Telephone.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                || reader.Address.Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                || reader.DateGet.ToShortDateString().Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                || reader.DateSet.ToShortDateString().Contains(filter_text, StringComparison.OrdinalIgnoreCase)
                ;
        }
    }
}
