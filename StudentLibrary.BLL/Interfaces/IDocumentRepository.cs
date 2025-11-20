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
