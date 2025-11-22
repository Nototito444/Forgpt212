using System;
using System.Collections.Generic;
using System.Linq;
using StudentLibrary.Core.Entities;
using StudentLibrary.BLL.Exceptions;
using StudentLibrary.Core.Interfaces;


namespace StudentLibrary.BLL.Services
{
    public class LibraryService : Interfaces.ILibraryService
    {
        private readonly IUserRepository _userRepo;
        private readonly IDocumentRepository _docRepo;
        private readonly int _maxBorrowLimit;

        public LibraryService(IUserRepository userRepo, IDocumentRepository docRepo, int maxBorrowLimit = 4)
        {
            _userRepo = userRepo;
            _docRepo = docRepo;
            _maxBorrowLimit = maxBorrowLimit; // n < 5 => максимум 4
        }

        // Users
        public User AddUser(string firstName, string lastName, string academicGroup)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Ім'я та прізвище не можуть бути порожніми");

            var user = new User(firstName.Trim(), lastName.Trim(), academicGroup?.Trim() ?? string.Empty);
            try
            {
                _userRepo.Add(user);
                var savedUser = _userRepo.GetById(user.Id);
                if (savedUser == null) throw new DataAccessException("Користувача не збережено");
                return savedUser;
            }
            catch (NotFoundException) { throw; }
            catch (Exception ex) { throw new DataAccessException("Помилка збереження користувача", ex); }
        }

        public void RemoveUser(Guid userId)
        {
            try
            {
                var user = _userRepo.GetById(userId);
                if (user == null) throw new NotFoundException("Користувача не знайдено");
                if (user.BorrowedCount > 0) throw new LimitExceededException("Користувач має невищенi книги, видалення заборонено");
                _userRepo.Remove(userId);
            }
            catch (NotFoundException) { throw; }
            catch (Exception ex) {throw new DataAccessException("Помилка при видаленні користувача", ex); }
        }

        public void UpdateUser(User user)
        {
            var exists = _userRepo.GetById(user.Id);
            if (exists == null) throw new NotFoundException("Користувача не знайдено");
            try
            {
                _userRepo.Update(user);
            }
            catch (NotFoundException) { throw; }
            catch (Exception ex) {throw new DataAccessException("Помилка при оновленні користувача", ex); }
        }

        public User GetUser(Guid userId)
        {
            var user = _userRepo.GetById(userId);
            if (user == null) throw new NotFoundException("Користувача не знайдено");
            return user;
        }

        public IEnumerable<User> GetAllUsers() => _userRepo.GetAll();

        // Documents
        public Document AddDocument(string title, string author, int year)
        {
            if (string.IsNullOrWhiteSpace(title)) throw new ArgumentException("Назва не може бути порожньою");
            var doc = new Document(title.Trim(), author?.Trim() ?? string.Empty, year);
            try
            {
                _docRepo.Add(doc);
                var savedDoc = _docRepo.GetById(doc.Id);
                if (savedDoc == null) throw new DataAccessException("Документ не збережено");
                return savedDoc;
            }
            catch (NotFoundException) { throw; }
            catch (Exception ex) { throw new DataAccessException("Помилка збереження документа", ex); }
        }

        public void RemoveDocument(Guid documentId)
        {
            var doc = _docRepo.GetById(documentId);
            if (doc == null) throw new NotFoundException("Документ не знайдено");
            if (!doc.IsAvailable) throw new InvalidOperationException("Документ видано, видалення заборонено");
            try
            {
                _docRepo.Remove(documentId);
            }
            catch (NotFoundException) { throw; }
            catch (Exception ex) { throw new DataAccessException("Помилка при видаленні документа", ex); }
        }

        public void UpdateDocument(Document document)
        {
            var exists = _docRepo.GetById(document.Id);
            if (exists == null) throw new NotFoundException("Документ не знайдено");
            try
            {
                _docRepo.Update(document);
            }
            catch (NotFoundException) { throw; }
            catch (Exception ex) { throw new DataAccessException("Помилка при оновленні документа", ex); }
        }

        public Document GetDocument(Guid documentId)
        {
            var doc = _docRepo.GetById(documentId);
            if (doc == null) throw new NotFoundException("Документ не знайдено");
            return doc;
        }

        public IEnumerable<Document> GetAllDocuments() => _docRepo.GetAll();

        // Lending
        public void IssueDocument(Guid documentId, Guid userId)
        {
            var user = _userRepo.GetById(userId);
            if (user == null) throw new NotFoundException("Користувача не знайдено");

            var doc = _docRepo.GetById(documentId);
            if (doc == null) throw new NotFoundException("Документ не знайдено");

            if (!doc.IsAvailable)
                throw new InvalidOperationException("Документ вже видано");

            if (user.BorrowedCount >= _maxBorrowLimit)
                throw new LimitExceededException($"Перевищено ліміт ({_maxBorrowLimit}) книг для одного абонементу");

            // обновляем модель
            doc.BorrowedByUserId = user.Id;
            user.AddBorrowedDocument(doc.Id);

            try
            {
                _docRepo.Update(doc);
                _userRepo.Update(user);
                
            }
            catch (NotFoundException) { throw; }
            catch (Exception ex) { throw new DataAccessException("Помилка при видачі документа", ex); }
        }

        public void ReturnDocument(Guid documentId, Guid userId)
        {
            var user = _userRepo.GetById(userId);
            if (user == null) throw new NotFoundException("Користувача не знайдено");

            var doc = _docRepo.GetById(documentId);
            if (doc == null) throw new NotFoundException("Документ не знайдено");

            if (doc.IsAvailable) throw new InvalidOperationException("Документ вже в бібліотеці");

            if (doc.BorrowedByUserId != user.Id) throw new InvalidOperationException("Документ не був виданий цьому користувачу");

            // обновляем модель
            doc.BorrowedByUserId = null;
            user.RemoveBorrowedDocument(doc.Id);

            try
            {
                _docRepo.Update(doc);
                _userRepo.Update(user);
               
            }
            catch (NotFoundException) { throw; }
            catch (Exception ex) { throw new DataAccessException("Помилка при поверненні документа", ex); }
        }

        public IEnumerable<Document> GetDocumentsBorrowedByUser(Guid userId)
        {
            var user = _userRepo.GetById(userId);
            if (user == null) throw new NotFoundException("Користувача не знайдено");

            var allDocs = _docRepo.GetAll();
            var result = allDocs.Where(d => d.BorrowedByUserId == userId);
            return result;
        }

        public (bool available, Guid? holderId) IsDocumentAvailable(Guid documentId)
        {
            var doc = _docRepo.GetById(documentId);
            if (doc == null) throw new NotFoundException("Документ не знайдено");
            return (doc.IsAvailable, doc.BorrowedByUserId);
        }

        // Search
        public IEnumerable<Document> SearchDocuments(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return GetAllDocuments();
            keyword = keyword.Trim().ToLowerInvariant();
            return _docRepo.GetAll().Where(d =>
                (!string.IsNullOrEmpty(d.Title) && d.Title.ToLowerInvariant().Contains(keyword)) ||
                (!string.IsNullOrEmpty(d.Author) && d.Author.ToLowerInvariant().Contains(keyword)));
        }

        public IEnumerable<User> SearchUsers(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword)) return GetAllUsers();
            keyword = keyword.Trim().ToLowerInvariant();
            return _userRepo.GetAll().Where(u =>
                (!string.IsNullOrEmpty(u.FirstName) && u.FirstName.ToLowerInvariant().Contains(keyword)) ||
                (!string.IsNullOrEmpty(u.LastName) && u.LastName.ToLowerInvariant().Contains(keyword)) ||
                (!string.IsNullOrEmpty(u.AcademicGroup) && u.AcademicGroup.ToLowerInvariant().Contains(keyword)));
        }

        // Sort
        public IEnumerable<User> GetUsersSortedByFirstName() => _userRepo.GetAll().OrderBy(u => u.FirstName);
        public IEnumerable<User> GetUsersSortedByLastName() => _userRepo.GetAll().OrderBy(u => u.LastName);
        public IEnumerable<User> GetUsersSortedByAcademicGroup() => _userRepo.GetAll().OrderBy(u => u.AcademicGroup);

        public IEnumerable<Document> GetDocumentsSortedByTitle() => _docRepo.GetAll().OrderBy(d => d.Title);
        public IEnumerable<Document> GetDocumentsSortedByAuthor() => _docRepo.GetAll().OrderBy(d => d.Author);
    }
}
