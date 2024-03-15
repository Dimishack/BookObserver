using BookObserver.Models.Readers;

namespace BookObserver.Models.Books
{
    class Book
    {
        public int Id { get; set; }
        public string Stock { get; set; } = "Нет";
        public int Pages { get; set; }
        public string YearPublish { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string BBK { get; set; } = string.Empty;
        public string CodeAuthor { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public string Publish { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Reader? Reader { get; set; }
    }
}
