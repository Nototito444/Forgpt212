using System;
using System.Collections.Generic;
using StudentLibrary.Core.Entities;

namespace StudentLibrary.Core.Interfaces
{
    public interface IUserRepository
    {
        void Add(User item);
        IEnumerable<User> GetAll();
        User? GetById(Guid id);
        void Remove(Guid id);
        void Update(User item);
        void SaveList();
    }
}