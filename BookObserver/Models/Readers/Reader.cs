using BookObserver.Models.Books;

namespace BookObserver.Models.Readers
{
    internal class Reader
    {
        public int Id { get; set; }
        public string FullName => LastName + FirstName + Patronymic;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string Patronymic { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateGet { get; set; }
        public DateTime DateSet { get; set; }
    }
}
