using System;
using System.Collections.Generic;
using System.Globalization;
using CinemaApp.Models;
using CinemaApp.Services;
using CinemaApp.UI;

namespace CinemaApp
{
    internal static class Program
    {
        private const string FilePath = "cinema_sessions.dat";

        private static readonly IReader Reader = new ConsoleReader();
        private static readonly IWriter Writer = new ConsoleWriter();
        private static readonly ISessionStorage Storage =
            new BinarySessionStorage(FilePath);
        private static SessionManager _manager = null!;

        private static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                var sessions = Storage.Load();
                _manager = new SessionManager(sessions);
                Writer.WriteLine($"Загружено записей: {_manager.Sessions.Count}");
            }
            catch (Exception ex)
            {
                Writer.WriteLine($"Ошибка загрузки БД: {ex.Message}");
                _manager = new SessionManager(new List<MovieSession>());
            }

            bool exit = false;
            while (!exit)
            {
                PrintMenu();
                int choice = ReadInt("Ваш выбор: ", 0, 8);
                try
                {
                    switch (choice)
                    {
                        case 0: exit = true; break;
                        case 1: ShowAll(); break;
                        case 2: AddSession(); break;
                        case 3: RemoveSession(); break;
                        case 4: QueryByGenre(); break;
                        case 5: QueryAffordable(); break;
                        case 6: QueryTotalRevenue(); break;
                        case 7: QueryAverageDuration(); break;
                        case 8: SaveDatabase(); break;
                    }
                }
                catch (Exception ex)
                {
                    Writer.WriteLine($"Ошибка: {ex.Message}");
                }
            }

            SaveDatabase();
            Writer.WriteLine("До свидания!");
        }

        private static void PrintMenu()
        {
            Writer.WriteLine("\n===== АФИША КИНОТЕАТРА =====");
            Writer.WriteLine("1. Просмотр базы данных");
            Writer.WriteLine("2. Добавить сеанс");
            Writer.WriteLine("3. Удалить сеанс по Id");
            Writer.WriteLine("4. Запрос: сеансы заданного жанра");
            Writer.WriteLine("5. Запрос: доступные сеансы (по дате и цене)");
            Writer.WriteLine("6. Запрос: общая выручка");
            Writer.WriteLine("7. Запрос: средняя длительность по жанру");
            Writer.WriteLine("8. Сохранить БД в файл");
            Writer.WriteLine("0. Выход");
        }

        private static void ShowAll()
        {
            if (_manager.Sessions.Count == 0)
            {
                Writer.WriteLine("База данных пуста.");
                return;
            }
            foreach (var s in _manager.Sessions)
                Writer.WriteLine(s.ToString());
        }

        private static void AddSession()
        {
            int id = ReadInt("Id: ", 1, int.MaxValue);
            if (_manager.ContainsId(id))
            {
                Writer.WriteLine("Запись с таким Id уже существует.");
                return;
            }

            string title = ReadNonEmptyString("Название фильма: ");
            string genre = ReadNonEmptyString("Жанр: ");
            DateTime start = ReadDateTime("Дата и время (дд.ММ.гггг ЧЧ:мм): ");
            int duration = ReadInt("Длительность (мин): ", 1, 600);
            decimal price = ReadDecimal("Цена билета: ", 0m, 100000m);
            int sold = ReadInt("Продано билетов: ", 0, 1000000);

            var session = new MovieSession(id, title, genre, start,
                duration, price, sold);
            _manager.Add(session);
            Writer.WriteLine("Сеанс добавлен.");
        }

        private static void RemoveSession()
        {
            int id = ReadInt("Id для удаления: ", 1, int.MaxValue);
            if (_manager.RemoveById(id))
                Writer.WriteLine("Сеанс удалён.");
            else
                Writer.WriteLine("Сеанс с таким Id не найден.");
        }

        private static void QueryByGenre()
        {
            string genre = ReadNonEmptyString("Жанр: ");
            var list = _manager.GetSessionsByGenre(genre);
            if (list.Count == 0)
            {
                Writer.WriteLine("Ничего не найдено.");
                return;
            }
            foreach (var s in list) Writer.WriteLine(s.ToString());
        }

        private static void QueryAffordable()
        {
            DateTime from = ReadDateTime("Начиная с (дд.ММ.гггг ЧЧ:мм): ");
            decimal max = ReadDecimal("Максимальная цена: ", 0m, 100000m);
            var list = _manager.GetAffordableSessionsAfter(from, max);
            if (list.Count == 0)
            {
                Writer.WriteLine("Ничего не найдено.");
                return;
            }
            foreach (var s in list) Writer.WriteLine(s.ToString());
        }

        private static void QueryTotalRevenue()
        {
            Writer.WriteLine(
                $"Общая выручка: {_manager.GetTotalRevenue():F2} руб.");
        }

        private static void QueryAverageDuration()
        {
            string genre = ReadNonEmptyString("Жанр: ");
            double avg = _manager.GetAverageDurationByGenre(genre);
            Writer.WriteLine($"Средняя длительность: {avg:F1} мин.");
        }

        private static void SaveDatabase()
        {
            Storage.Save(new List<MovieSession>(_manager.Sessions));
            Writer.WriteLine("БД сохранена.");
        }

        private static string ReadNonEmptyString(string prompt)
        {
            while (true)
            {
                string? input = Reader.ReadLine(prompt);
                if (!string.IsNullOrWhiteSpace(input)) return input.Trim();
                Writer.WriteLine("Значение не может быть пустым. Повторите.");
            }
        }

        private static int ReadInt(string prompt, int min, int max)
        {
            while (true)
            {
                string? input = Reader.ReadLine(prompt);
                if (int.TryParse(input, NumberStyles.Integer,
                        CultureInfo.InvariantCulture, out int value)
                    && value >= min && value <= max)
                {
                    return value;
                }
                Writer.WriteLine(
                    $"Введите целое число в диапазоне [{min}; {max}].");
            }
        }

        private static decimal ReadDecimal(string prompt, decimal min, decimal max)
        {
            while (true)
            {
                string? input = Reader.ReadLine(prompt);
                if (decimal.TryParse(input, NumberStyles.Number,
                        CultureInfo.InvariantCulture, out decimal value)
                    && value >= min && value <= max)
                {
                    return value;
                }
                Writer.WriteLine($"Введите число в диапазоне [{min}; {max}].");
            }
        }

        private static DateTime ReadDateTime(string prompt)
        {
            while (true)
            {
                string? input = Reader.ReadLine(prompt);
                if (DateTime.TryParseExact(input, "dd.MM.yyyy HH:mm",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime value))
                {
                    return value;
                }
                Writer.WriteLine("Неверный формат. Пример: 25.12.2025 19:30");
            }
        }
    }
}