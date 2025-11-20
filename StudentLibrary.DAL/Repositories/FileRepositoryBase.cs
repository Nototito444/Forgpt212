using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StudentLibrary.DAL.Repositories
{
    // Базовый репозиторий: хранение коллекции T в JSON-файле
    public abstract class FileRepositoryBase<T> where T : class
    {
        private readonly string _filePath;
        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();

        protected FileRepositoryBase(string filePath)
        {
            _filePath = filePath;
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (!File.Exists(_filePath))
            {
                SaveList(new List<T>());
            }
        }

        protected List<T> LoadList()
        {
            _locker.EnterReadLock();
            try
            {
                var json = File.ReadAllText(_filePath);
                if (string.IsNullOrWhiteSpace(json)) return new List<T>();
                return JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                       ?? new List<T>();
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        protected void SaveList(List<T> list)
        {
            _locker.EnterWriteLock();
            try
            {
                var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }
    }
}
