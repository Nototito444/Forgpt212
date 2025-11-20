using System;

namespace StudentLibrary.PL.Helpers
{
    public static class ConsoleHelper
    {
        public static string ReadRequired(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input)) return input.Trim();
                Console.WriteLine("Поле не може бути порожнім. Спробуйте ще раз.");
            }
        }

        public static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (int.TryParse(input, out var v)) return v;
                Console.WriteLine("Введіть коректне число.");
            }
        }

        public static Guid ReadGuid(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (Guid.TryParse(input, out var g)) return g;
                Console.WriteLine("Введіть коректний GUID.");
            }
        }

        public static void Pause()
        {
            Console.WriteLine("Натисніть Enter для продовження...");
            Console.ReadLine();
        }
    }
}
