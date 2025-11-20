using System;
using System.Collections.Generic;

namespace StudentLibrary.Core.Entities
{
    /// <summary>
    /// Представление пользователя (читателя)
    /// </summary>
    public class User
    {
        public Guid Id { get; private set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string AcademicGroup { get; set; } = string.Empty;

        // Храним Id выданных документов
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

        // Добавить документ в список выданных
        public void AddBorrowedDocument(Guid documentId)
        {
            if (!_borrowedDocumentIds.Contains(documentId))
                _borrowedDocumentIds.Add(documentId);
        }

        // Удалить документ при возврате
        public bool RemoveBorrowedDocument(Guid documentId)
        {
            return _borrowedDocumentIds.Remove(documentId);
        }

        public int BorrowedCount => _borrowedDocumentIds.Count;
    }
}
