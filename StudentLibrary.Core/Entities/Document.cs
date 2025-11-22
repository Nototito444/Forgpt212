using System;
using System.Text.Json.Serialization;

namespace StudentLibrary.Core.Entities
{
    public class Document
    {
        [JsonInclude] public Guid Id { get; private set; }
        [JsonInclude] public string Title { get; set; } = string.Empty;
        [JsonInclude] public string Author { get; set; } = string.Empty;
        [JsonInclude] public int Year { get; set; }

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
