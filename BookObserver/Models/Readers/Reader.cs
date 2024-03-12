using BookObserver.Models.Books;
using System;

namespace BookObserver.Models.Readers
{
    internal class Reader
    {
        public int Id { get; set; }
        public string? FullName => LastName + FirstName + Patronymic;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Patronymic { get; set; }
        public string? Telephone { get; set; }
        public string? Address { get; set; }
        public DateTime DateGet { get; set; }
        public DateTime DateSet { get; set; }
        public Book? Book { get; set; }
    }
}
