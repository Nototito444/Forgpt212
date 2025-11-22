using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace StudentLibrary.Core.Entities
{
    public class User
    {
        [JsonInclude] public Guid Id {get; private set; }
        [JsonInclude] public string FirstName { get; set; } = string.Empty;
        [JsonInclude] public string LastName  { get; set; } = string.Empty;
        [JsonInclude] public string AcademicGroup { get; set; } = string.Empty;
        [JsonInclude] public List<Guid> BorrowedDocumentIds { get; private set; } = new List<Guid>();

        public int BorrowedCount => BorrowedDocumentIds.Count;

        public User()
        {
            Id = Guid.NewGuid();
        }

        public User(string firstName, string lastName, string academicGroup) : this()
        {
            FirstName = firstName;
            LastName = lastName;
            AcademicGroup = academicGroup;
        }

        public void AddBorrowedDocument(Guid documentId)
        {
            if (!BorrowedDocumentIds.Contains(documentId))
                BorrowedDocumentIds.Add(documentId);
        }
        public bool RemoveBorrowedDocument(Guid documentId)
        {
            return BorrowedDocumentIds.Remove(documentId);
        }

    }
}
