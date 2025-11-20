using System;

namespace StudentLibrary.Core.Entities
{
    public class Document
    {
        public Guid Id { get; private set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public int Year { get; set; }

        // Если документ выдан - хранится Id читателя, иначе null
        public Guid? BorrowedByUserId { get; set; } = null;

        public Document()
        {
            Id = Guid.NewGuid();
        }

        public Document(string title, string author, int year) : this()
        {
            Title = title;
            Author = author;
            Year = year;
        }

        public bool IsAvailable => BorrowedByUserId == null;
    }
}
