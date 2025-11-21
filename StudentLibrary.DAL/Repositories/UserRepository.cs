using StudentLibrary.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using StudentLibrary.Core.Interfaces;


namespace StudentLibrary.DAL.Repositories
{
    public class UserRepository : FileRepositoryBase<User>, IUserRepository
    {
        public UserRepository(string filePath) : base(filePath)
        {
        }

        public void Add(User item)
        {
            var list = LoadList();
            list.Add(item);
            SaveList(list);
        }

        public IEnumerable<User> GetAll()
        {
            return LoadList();
        }

        public User? GetById(Guid id)
        {
            return LoadList().FirstOrDefault(u => u.Id == id);
        }

        public void Remove(Guid id)
        {
            var list = LoadList();
            var removed = list.RemoveAll(u => u.Id == id);
            if (removed > 0) SaveList(list);
        }

        public void Update(User item)
        {
            var list = LoadList();
            var idx = list.FindIndex(u => u.Id == item.Id);
            if (idx >= 0)
            {
                list[idx] = item;
                SaveList(list);
            }
        }
    
public void SaveList() 
    {
        SaveList(LoadList()); 
    }
    }
}

