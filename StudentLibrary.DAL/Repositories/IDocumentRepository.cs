using StudentLibrary.BLL.Entities;
using System;

namespace StudentLibrary.DAL.Repositories
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        // дополнительные методы специфичные для документов
    }
}
