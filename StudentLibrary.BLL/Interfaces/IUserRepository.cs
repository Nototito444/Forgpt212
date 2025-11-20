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