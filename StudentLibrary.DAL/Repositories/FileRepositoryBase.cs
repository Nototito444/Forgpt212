using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace StudentLibrary.DAL.Repositories
{
    public abstract class FileRepositoryBase<T> where T : class
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        private readonly object _fileLock;

        protected FileRepositoryBase(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("filePath must be provided", nameof(filePath));

            _filePath = Path.GetFullPath(filePath);
            _fileLock = (_filePath).Intern(); 

            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        protected List<T> LoadList()
        {
            lock (_fileLock)
            {
                var json = File.ReadAllText(_filePath);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<T>();

                try
                {
                    var list = JsonSerializer.Deserialize<List<T>>(json, _jsonOptions);
                    return list ?? new List<T>();
                }
                catch
                {
                    return new List<T>();
                }
            }
        }

        protected void SaveList(List<T> list)
        {
            lock (_fileLock)
            {
                var json = JsonSerializer.Serialize(list, _jsonOptions);
                File.WriteAllText(_filePath, json);
            }
        }
    }

    internal static class StringLockExtensions
    {
        public static object Intern(this string s) => string.Intern(s);
    }
}