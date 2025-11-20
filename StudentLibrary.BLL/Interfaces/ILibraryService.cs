using System;
using System.Collections.Generic;
using StudentLibrary.Core.Entities;
using StudentLibrary.DAL.Repositories;

namespace StudentLibrary.BLL.Interfaces
{
    public interface ILibraryService
    {
        // Users
        User AddUser(string firstName, string lastName, string academicGroup);
        void RemoveUser(Guid userId);
        void UpdateUser(User user);
        User GetUser(Guid userId);
        IEnumerable<User> GetAllUsers();

        // Documents
        Document AddDocument(string title, string author, int year);
        void RemoveDocument(Guid documentId);
        void UpdateDocument(Document document);
        Document GetDocument(Guid documentId);
        IEnumerable<Document> GetAllDocuments();

        // Lending
        void IssueDocument(Guid documentId, Guid userId);    // выдать
        void ReturnDocument(Guid documentId, Guid userId);   // вернуть
        IEnumerable<Document> GetDocumentsBorrowedByUser(Guid userId);
        (bool available, Guid? holderId) IsDocumentAvailable(Guid documentId);

        // Search
        IEnumerable<Document> SearchDocuments(string keyword);
        IEnumerable<User> SearchUsers(string keyword);

        // Sort
        IEnumerable<User> GetUsersSortedByFirstName();
        IEnumerable<User> GetUsersSortedByLastName();
        IEnumerable<User> GetUsersSortedByAcademicGroup();

        IEnumerable<Document> GetDocumentsSortedByTitle();
        IEnumerable<Document> GetDocumentsSortedByAuthor();
    }
}
