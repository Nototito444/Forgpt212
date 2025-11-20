// BLL/Interfaces/IUserRepository.cs
using System;
using System.Collections.Generic;
using StudentLibrary.BLL.Entities;

namespace StudentLibrary.BLL.Interfaces
{
    public interface IUserRepository
    {
        void Add(User item);
        IEnumerable<User> GetAll();
        User? GetById(Guid id);
        void Remove(Guid id);
        void Update(User item);
    }
}

// BLL/Interfaces/IDocumentRepository.cs
using System;
using System.Collections.Generic;
using StudentLibrary.BLL.Entities;

namespace StudentLibrary.BLL.Interfaces
{
    public interface IDocumentRepository
    {
        void Add(Document item);
        IEnumerable<Document> GetAll();
        Document? GetById(Guid id);
        void Remove(Guid id);
        void Update(Document item);
    }
}
