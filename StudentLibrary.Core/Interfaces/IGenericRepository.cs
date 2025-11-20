using System;
using System.Collections.Generic;
using StudentLibrary.Core.Entities;

namespace StudentLibrary.Core.Repositories
{
    public interface IGenericRepository<T>
    {
        IEnumerable<T> GetAll();
        T? GetById(Guid id);
        void Add(T item);
        void Update(T item);
        void Remove(Guid id);
    }
}
