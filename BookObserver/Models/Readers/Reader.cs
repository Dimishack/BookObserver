using System.Collections.ObjectModel;

namespace BookObserver.Models.Readers
{
    internal class Reader
    {
        public int Id { get; set; }
        public string BooksWithHim { get; set; } = "Нет";
        public string FullName => $"{LastName} {FirstName} {Patronymic}";
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string HomePhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public ObservableCollection<int> ListIdBook { get; set; } = [];
        public ObservableCollection<string> AuthorAndNameBook { get; set; } = [];
    }
}
