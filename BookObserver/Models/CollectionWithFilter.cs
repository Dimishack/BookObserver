using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BookObserver.Models
{
    internal class CollectionWithFilter : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #region List : IList<string> - Список

        ///<summary>Список</summary>
        private IList<string> _list;

        /// <summary>Список</summary>
        public IList<string> List
        {
            set
            {
                if (_list.SequenceEqual(value)) return;
                _list = value;
                if (_collectionView is not null)
                {
                    CollectionView = null;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
                CollectionView = new(value);
                OnPropertyChanged();
            }
        }

        #endregion

        #region CollectionView : ObservableCollection<T> - Вывод списка

        ///<summary>Вывод списка</summary>
        private ObservableCollection<string>? _collectionView;

        ///<summary>Вывод списка</summary>
        public ObservableCollection<string>? CollectionView
        {
            get => _collectionView;
            private set
            {
                if (_collectionView == value) return;
                _collectionView = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public CollectionWithFilter() => _list = [];

        public void RefreshFilter(string filter)
        {
            var test = _list.Where(l => l.Contains(filter)).ToList();
            if (_collectionView is null || _collectionView.SequenceEqual(test)) return;

            CollectionView = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            CollectionView = new(test);
        }
    }
}
