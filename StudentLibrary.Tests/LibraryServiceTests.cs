using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using StudentLibrary.BLL.Entities;
using StudentLibrary.BLL.Services;
using StudentLibrary.DAL.Repositories;
using StudentLibrary.BLL.Exceptions;

namespace StudentLibrary.Tests
{
    [TestFixture]
    public class LibraryServiceTests
    {
        private string _tmpDir = Path.Combine(Path.GetTempPath(), "StudentLibraryTests");
        private string _usersFile => Path.Combine(_tmpDir, "users.json");
        private string _docsFile => Path.Combine(_tmpDir, "documents.json");
        private UserRepository _userRepo!;
        private DocumentRepository _docRepo!;
        private LibraryService _service!;

        [SetUp]
        public void Setup()
        {
            if (Directory.Exists(_tmpDir)) Directory.Delete(_tmpDir, true);
            Directory.CreateDirectory(_tmpDir);
            _userRepo = new UserRepository(_usersFile);
            _docRepo = new DocumentRepository(_docsFile);
            _service = new LibraryService(_userRepo, _docRepo, maxBorrowLimit: 4);
        }

        [Test]
        public void AddUser_And_GetUser_ShouldWork()
        {
            var u = _service.AddUser("Ivan", "Ivanov", "KS-1");
            var got = _service.GetUser(u.Id);
            got.FirstName.Should().Be("Ivan");
            got.LastName.Should().Be("Ivanov");
            got.AcademicGroup.Should().Be("KS-1");
        }

        [Test]
        public void AddDocument_And_GetDocument_ShouldWork()
        {
            var d = _service.AddDocument("Title", "Author", 2020);
            var got = _service.GetDocument(d.Id);
            got.Title.Should().Be("Title");
            got.Author.Should().Be("Author");
        }

        [Test]
        public void IssueDocument_ShouldRespectLimit()
        {
            var u = _service.AddUser("A", "B", "G1");
            var docs = Enumerable.Range(1, 5).Select(i => _service.AddDocument($"T{i}", $"A{i}", 2000 + i)).ToList();

            // give up to max (4)
            for (int i = 0; i < 4; i++)
            {
                _service.IssueDocument(docs[i].Id, u.Id);
            }

            // 5th should throw LimitExceededException
            Action act = () => _service.IssueDocument(docs[4].Id, u.Id);
            act.Should().Throw<LimitExceededException>();
        }

        [Test]
        public void IssueAndReturn_ShouldUpdateStates()
        {
            var u = _service.AddUser("U", "L", "G");
            var d = _service.AddDocument("T", "A", 2010);

            _service.IssueDocument(d.Id, u.Id);
            var docAfterIssue = _service.GetDocument(d.Id);
            docAfterIssue.IsAvailable.Should().BeFalse();

            var borrowed = _service.GetDocumentsBorrowedByUser(u.Id);
            borrowed.Count().Should().Be(1);

            _service.ReturnDocument(d.Id, u.Id);
            var docAfterReturn = _service.GetDocument(d.Id);
            docAfterReturn.IsAvailable.Should().BeTrue();
            _service.GetDocumentsBorrowedByUser(u.Id).Any().Should().BeFalse();
        }

        [Test]
        public void SearchUsers_ShouldFindByKeyword()
        {
            _service.AddUser("Anna", "Smith", "G1");
            _service.AddUser("Ivan", "Petrov", "G2");

            var res = _service.SearchUsers("Ann");
            res.Count().Should().Be(1);
            res.First().FirstName.Should().Be("Anna");
        }
    }
}
