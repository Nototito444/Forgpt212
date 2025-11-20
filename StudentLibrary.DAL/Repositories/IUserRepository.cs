// DAL/Repositories/IUserRepository.cs
using StudentLibrary.BLL.Entities;
using System;

namespace StudentLibrary.DAL.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Методы DAL-уровня, если нужны (например, загрузка/сохранение в файле)
    }
}

// DAL/Repositories/IDocumentRepository.cs
using StudentLibrary.BLL.Entities;
using System;

namespace StudentLibrary.DAL.Repositories
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        // Методы DAL-уровня
    }
}
