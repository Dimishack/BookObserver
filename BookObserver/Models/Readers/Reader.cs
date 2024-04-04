using System.Collections.ObjectModel;

namespace BookObserver.Models.Readers
{
    internal class Reader
    {
        public int Id { get; set; }
        public string FullName => $"{LastName} {FirstName} {Patronymic}";
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public string NumberPhone { get; set; } = string.Empty;
        public string HomeNumberPhone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public ObservableCollection<int> ListIdBook { get; set; } = new();
    }
}
