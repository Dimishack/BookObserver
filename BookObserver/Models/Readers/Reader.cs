using BookObserver.Models.Books;

namespace BookObserver.Models.Readers
{
    internal class Reader
    {
        public int Id { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Patronymic { get; set; }
        public string? Telephone { get; set; }
        public string? Address { get; set; }
        public Book? Book { get; set; }
    }
}
