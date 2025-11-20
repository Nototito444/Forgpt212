using System;
using System.Collections.Generic;
using StudentLibrary.Core.Entities;


namespace StudentLibrary.Core.Interfaces
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
