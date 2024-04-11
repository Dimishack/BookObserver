namespace BookObserver.Models.Books
{
    class Book
    {
        public int Id { get; set; }
        public int? IndexReader { get; set; }
        public string? FullNameReader { get; set; }
        public string Existence { get; set; } = "Да";
        public string BBK { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Publish { get; set; } = string.Empty;
        public string YearPublish { get; set; } = string.Empty;
        public string Pages { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public DateTime? DateGet { get; set; }
        public DateTime? DateSet { get; set; }
    }
}
