using StudentLibrary.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using StudentLibrary.Core.Interfaces;

namespace StudentLibrary.DAL.Repositories
{
    public class DocumentRepository : FileRepositoryBase<Document>, IDocumentRepository
    {
        public DocumentRepository(string filePath) : base(filePath)
        {
        }

        public void Add(Document item)
        {
            var list = LoadList();
            list.Add(item);
            SaveList(list);
        }

        public IEnumerable<Document> GetAll()
        {
            return LoadList();
        }

        public Document? GetById(Guid id)
        {
            return LoadList().FirstOrDefault(d => d.Id == id);
        }

        public void Remove(Guid id)
        {
            var list = LoadList();
            var removed = list.RemoveAll(d => d.Id == id);
            if (removed > 0) SaveList(list);
        }

        public void Update(Document item)
        {
            var list = LoadList();
            var idx = list.FindIndex(d => d.Id == item.Id);
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