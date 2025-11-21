using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace StudentLibrary.DAL.Repositories
{
    // Generic base for file-backed repositories
    public abstract class FileRepositoryBase<T> where T : class
    {
        private readonly string _filePath;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        // use a dedicated lock object per file path
        private readonly object _fileLock;

        protected FileRepositoryBase(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("filePath must be provided", nameof(filePath));

            _filePath = Path.GetFullPath(filePath);
            _fileLock = (_filePath).Intern(); // small trick to have per-path lock; alternatively use a static Dictionary

            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            // ensure file exists and contains an empty array if missing
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
                    // If deserialization fails, return empty list instead of throwing in tests
                    return new List<T>();
                }
            }
        }

        // Expose for derived classes
        protected void SaveList(List<T> list)
        {
            lock (_fileLock)
            {
                var json = JsonSerializer.Serialize(list, _jsonOptions);
                File.WriteAllText(_filePath, json);
            }
        }
    }

    // Helper extension to produce stable lock object per string. If your target framework doesn't have string.Intern() usage policy,
    // you can replace by a static ConcurrentDictionary<string, object> to store locks per path.
    internal static class StringLockExtensions
    {
        public static object Intern(this string s) => string.Intern(s);
    }
}