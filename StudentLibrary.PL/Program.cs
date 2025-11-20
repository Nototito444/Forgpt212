using System;
using System.Linq;
using StudentLibrary.BLL.Services;
using StudentLibrary.DAL.Repositories;
using StudentLibrary.PL.Helpers;
using StudentLibrary.BLL.Interfaces;
using StudentLibrary.Core.Entities;

namespace StudentLibrary.PL
{
    class Program
    {
        static void Main(string[] args)
        {
            // Настройки путей к данным (в папке проекта при запуске)
            var baseDataDir = "Data";
            var usersFile = System.IO.Path.Combine(baseDataDir, "users.json");
            var docsFile = System.IO.Path.Combine(baseDataDir, "documents.json");

            // Создаём репозитории и сервис
            var userRepo = new UserRepository(usersFile);
            var docRepo = new DocumentRepository(docsFile);
            ILibraryService service = new LibraryService(userRepo, docRepo);

            Console.WriteLine("=== Студентська бібліотека (консоль) ===");
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Виберіть опцію:");
                Console.WriteLine("1. Керування користувачами");
                Console.WriteLine("2. Керування документами");
                Console.WriteLine("3. Видання / Повернення");
                Console.WriteLine("4. Пошук");
                Console.WriteLine("5. Перегляд списків та сортування");
                Console.WriteLine("0. Вихід");
                var choice = ConsoleHelper.ReadRequired("-> ");

                try
                {
                    if (choice == "0") break;
                    switch (choice)
                    {
                        case "1": ManageUsers(service); break;
                        case "2": ManageDocuments(service); break;
                        case "3": ManageLending(service); break;
                        case "4": ManageSearch(service); break;
                        case "5": ManageLists(service); break;
                        default: Console.WriteLine("Невірна опція."); break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка: {ex.Message}");
                }

                ConsoleHelper.Pause();
            }
        }

        static void ManageUsers(ILibraryService service)
        {
            Console.WriteLine("Керування користувачами:");
            Console.WriteLine("1. Додати користувача");
            Console.WriteLine("2. Видалити користувача");
            Console.WriteLine("3. Змінити дані користувача");
            Console.WriteLine("4. Переглянути дані користувача");
            Console.WriteLine("5. Переглянути список всіх користувачів");
            var ch = ConsoleHelper.ReadRequired("-> ");
            switch (ch)
            {
                case "1":
                    var fn = ConsoleHelper.ReadRequired("Ім'я: ");
                    var ln = ConsoleHelper.ReadRequired("Прізвище: ");
                    var grp = ConsoleHelper.ReadRequired("Академічна група: ");
                    var u = service.AddUser(fn, ln, grp);
                    Console.WriteLine($"Користувача додано. Id: {u.Id}");
                    break;
                case "2":
                    var idToDel = ConsoleHelper.ReadGuid("Id користувача: ");
                    service.RemoveUser(idToDel);
                    Console.WriteLine("Користувача видалено.");
                    break;
                case "3":
                    var idToUpd = ConsoleHelper.ReadGuid("Id користувача: ");
                    var user = service.GetUser(idToUpd);
                    Console.WriteLine($"Поточне ім'я: {user.FirstName}");
                    user.FirstName = ConsoleHelper.ReadRequired("Нове ім'я: ");
                    user.LastName = ConsoleHelper.ReadRequired("Нове прізвище: ");
                    user.AcademicGroup = ConsoleHelper.ReadRequired("Нова академічна група: ");
                    service.UpdateUser(user);
                    Console.WriteLine("Оновлено.");
                    break;
                case "4":
                    var idToView = ConsoleHelper.ReadGuid("Id користувача: ");
                    var uview = service.GetUser(idToView);
                    Console.WriteLine($"Id: {uview.Id}");
                    Console.WriteLine($"Ім'я: {uview.FirstName}");
                    Console.WriteLine($"Прізвище: {uview.LastName}");
                    Console.WriteLine($"Група: {uview.AcademicGroup}");
                    Console.WriteLine($"Кількість виданих документів: {uview.BorrowedCount}");
                    if (uview.BorrowedCount > 0)
                    {
                        var docs = service.GetDocumentsBorrowedByUser(uview.Id);
                        foreach (var d in docs) Console.WriteLine($" - {d.Title} ({d.Id})");
                    }
                    break;
                case "5":
                    var all = service.GetAllUsers();
                    foreach (var uu in all) Console.WriteLine($"{uu.Id} - {uu.FirstName} {uu.LastName} ({uu.AcademicGroup}) [{uu.BorrowedCount}]");
                    break;
                default:
                    Console.WriteLine("Невірна опція");
                    break;
            }
        }

        static void ManageDocuments(ILibraryService service)
        {
            Console.WriteLine("Керування документами:");
            Console.WriteLine("1. Додати документ");
            Console.WriteLine("2. Видалити документ");
            Console.WriteLine("3. Змінити документ");
            Console.WriteLine("4. Переглянути документ");
            Console.WriteLine("5. Переглянути всі документи");
            var ch = ConsoleHelper.ReadRequired("-> ");
            switch (ch)
            {
                case "1":
                    var title = ConsoleHelper.ReadRequired("Назва: ");
                    var author = ConsoleHelper.ReadRequired("Автор: ");
                    var year = ConsoleHelper.ReadInt("Рік: ");
                    var d = service.AddDocument(title, author, year);
                    Console.WriteLine($"Документ додано. Id: {d.Id}");
                    break;
                case "2":
                    var idDel = ConsoleHelper.ReadGuid("Id документа: ");
                    service.RemoveDocument(idDel);
                    Console.WriteLine("Документ видалено.");
                    break;
                case "3":
                    var idUpd = ConsoleHelper.ReadGuid("Id документа: ");
                    var doc = service.GetDocument(idUpd);
                    doc.Title = ConsoleHelper.ReadRequired($"Нова назва ({doc.Title}): ");
                    doc.Author = ConsoleHelper.ReadRequired($"Новий автор ({doc.Author}): ");
                    doc.Year = ConsoleHelper.ReadInt($"Новий рік ({doc.Year}): ");
                    service.UpdateDocument(doc);
                    Console.WriteLine("Оновлено.");
                    break;
                case "4":
                    var idView = ConsoleHelper.ReadGuid("Id документа: ");
                    var dv = service.GetDocument(idView);
                    Console.WriteLine($"{dv.Id} - {dv.Title} by {dv.Author} ({dv.Year})");
                    Console.WriteLine(dv.IsAvailable ? "У наявності" : $"Видано користувачу: {dv.BorrowedByUserId}");
                    break;
                case "5":
                    var allDocs = service.GetAllDocuments();
                    foreach (var doci in allDocs) Console.WriteLine($"{documentDisplay(doci)}");
                    break;
                default:
                    Console.WriteLine("Невірна опція");
                    break;
            }

            static string documentDisplay(Document d) =>
                $"{d.Id} - {d.Title} by {d.Author} ({d.Year}) - {(d.IsAvailable ? "в наявності" : $"видано {d.BorrowedByUserId}")}";
        }

        static void ManageLending(ILibraryService service)
        {
            Console.WriteLine("Видання / Повернення:");
            Console.WriteLine("1. Видати документ");
            Console.WriteLine("2. Повернути документ");
            Console.WriteLine("3. Переглянути, які документи взяв користувач");
            Console.WriteLine("4. Перевірити наявність документа");
            var ch = ConsoleHelper.ReadRequired("-> ");
            switch (ch)
            {
                case "1":
                    var docId = ConsoleHelper.ReadGuid("Id документа: ");
                    var userId = ConsoleHelper.ReadGuid("Id користувача: ");
                    service.IssueDocument(docId, userId);
                    Console.WriteLine("Документ видано.");
                    break;
                case "2":
                    var docIdR = ConsoleHelper.ReadGuid("Id документа: ");
                    var userIdR = ConsoleHelper.ReadGuid("Id користувача: ");
                    service.ReturnDocument(docIdR, userIdR);
                    Console.WriteLine("Документ повернено.");
                    break;
                case "3":
                    var uId = ConsoleHelper.ReadGuid("Id користувача: ");
                    var docs = service.GetDocumentsBorrowedByUser(uId);
                    foreach (var d in docs) Console.WriteLine($"{d.Id} - {d.Title} by {d.Author}");
                    break;
                case "4":
                    var checkId = ConsoleHelper.ReadGuid("Id документа: ");
                    var (available, holder) = service.IsDocumentAvailable(checkId);
                    Console.WriteLine(available ? "Документ у бібліотеці" : $"Документ видано користувачу: {holder}");
                    break;
                default:
                    Console.WriteLine("Невірна опція");
                    break;
            }
        }

        static void ManageSearch(ILibraryService service)
        {
            Console.WriteLine("Пошук:");
            Console.WriteLine("1. Пошук серед документів");
            Console.WriteLine("2. Пошук серед користувачів");
            var ch = ConsoleHelper.ReadRequired("-> ");
            switch (ch)
            {
                case "1":
                    var kw = ConsoleHelper.ReadRequired("Ключове слово: ");
                    var docs = service.SearchDocuments(kw);
                    foreach (var d in docs) Console.WriteLine($"{d.Id} - {d.Title} by {d.Author}");
                    break;
                case "2":
                    var kw2 = ConsoleHelper.ReadRequired("Ключове слово: ");
                    var users = service.SearchUsers(kw2);
                    foreach (var u in users) Console.WriteLine($"{u.Id} - {u.FirstName} {u.LastName} ({u.AcademicGroup})");
                    break;
                default:
                    Console.WriteLine("Невірна опція");
                    break;
            }
        }

        static void ManageLists(ILibraryService service)
        {
            Console.WriteLine("Сортування користувачів:");
            Console.WriteLine("1. По імені");
            Console.WriteLine("2. По прізвищу");
            Console.WriteLine("3. По академічній групі");
            Console.WriteLine("4. Документи по назві");
            Console.WriteLine("5. Документи по автору");
            var ch = ConsoleHelper.ReadRequired("-> ");
            switch (ch)
            {
                case "1":
                    foreach (var u in service.GetUsersSortedByFirstName()) Console.WriteLine($"{u.FirstName} {u.LastName} - {u.AcademicGroup} ({u.Id})");
                    break;
                case "2":
                    foreach (var u in service.GetUsersSortedByLastName()) Console.WriteLine($"{u.LastName} {u.FirstName} - {u.AcademicGroup} ({u.Id})");
                    break;
                case "3":
                    foreach (var u in service.GetUsersSortedByAcademicGroup()) Console.WriteLine($"{u.AcademicGroup} - {u.FirstName} {u.LastName} ({u.Id})");
                    break;
                case "4":
                    foreach (var d in service.GetDocumentsSortedByTitle()) Console.WriteLine($"{d.Title} by {d.Author} ({d.Id})");
                    break;
                case "5":
                    foreach (var d in service.GetDocumentsSortedByAuthor()) Console.WriteLine($"{d.Author} - {d.Title} ({d.Id})");
                    break;
                default:
                    Console.WriteLine("Невірна опція");
                    break;
            }
        }
    }
}
