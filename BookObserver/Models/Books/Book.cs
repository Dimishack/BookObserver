using BookObserver.Models.Readers;

namespace BookObserver.Models.Books
{
    class Book
    {
        public int Id { get; set; }
        public string? Stock { get; set; }
        public int Pages { get; set; }
        public string? YearPublish { get; set; }
        public string? Author { get; set; }
        public string? BBK { get; set; }
        public string? CodeAuthor { get; set; }
        public string? ISBN { get; set; }
        public string? Publish { get; set; }
        public string? Name { get; set; }
        public Reader? Reader { get; set; }
    }
}
