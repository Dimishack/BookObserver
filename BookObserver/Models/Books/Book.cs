using BookObserver.Models.Readers;

namespace BookObserver.Models.Books
{
    class Book
    {
        public int Id { get; set; }
        public string? BBK {  get; set; }
        public string? Publish { get; set; }
        public int? YearPublish { get; set; }
        public int? Pages { get; set; }
        public string? Author { get; set; }
        public string? Name { get; set; }
        public bool Stock {  get; set; }
        public Reader? Reader { get; set; }
    }
}
