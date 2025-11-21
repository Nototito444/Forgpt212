using System;
using System.Collections.Generic;

namespace StudentLibrary.Core.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string AcademicGroup { get; set; } = string.Empty;

        private readonly List<Guid> _borrowedDocumentIds = new List<Guid>();
        public IReadOnlyList<Guid> BorrowedDocumentIds => _borrowedDocumentIds.AsReadOnly();

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
            if (!_borrowedDocumentIds.Contains(documentId))
                _borrowedDocumentIds.Add(documentId);
        }

        public bool RemoveBorrowedDocument(Guid documentId)
        {
            return _borrowedDocumentIds.Remove(documentId);
        }

        public int BorrowedCount => _borrowedDocumentIds.Count;
    }
}
