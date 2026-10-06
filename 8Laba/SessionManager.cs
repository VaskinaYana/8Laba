using System;
using System.Collections.Generic;
using System.Linq;
using CinemaApp.Models;

namespace CinemaApp.Services
{
    public class SessionManager
    {
        private readonly List<MovieSession> _sessions;

        public SessionManager(List<MovieSession> sessions)
        {
            _sessions = sessions ?? new List<MovieSession>();
        }

        public IReadOnlyList<MovieSession> Sessions => _sessions;

        public bool RemoveById(int id)
        {
            var item = _sessions.FirstOrDefault(s => s.Id == id);
            if (item == null) return false;
            _sessions.Remove(item);
            return true;
        }

        public void Add(MovieSession session)
        {
            if (_sessions.Any(s => s.Id == session.Id))
                throw new InvalidOperationException(
                    $"Сеанс с Id={session.Id} уже существует.");
            _sessions.Add(session);
        }

        public bool ContainsId(int id) => _sessions.Any(s => s.Id == id);

        public List<MovieSession> GetSessionsByGenre(string genre)
        {
            return (from s in _sessions
                    where string.Equals(s.Genre, genre,
                        StringComparison.OrdinalIgnoreCase)
                    orderby s.StartTime
                    select s).ToList();
        }

        public List<MovieSession> GetAffordableSessionsAfter(
            DateTime from, decimal maxPrice)
        {
            return _sessions
                .Where(s => s.StartTime >= from && s.TicketPrice <= maxPrice)
                .OrderBy(s => s.TicketPrice)
                .ThenBy(s => s.StartTime)
                .ToList();
        }

        public decimal GetTotalRevenue()
        {
            return _sessions.Sum(s => s.TicketPrice * s.TicketsSold);
        }

        public double GetAverageDurationByGenre(string genre)
        {
            var query = from s in _sessions
                        where string.Equals(s.Genre, genre,
                            StringComparison.OrdinalIgnoreCase)
                        select s.DurationMinutes;

            return query.Any() ? query.Average() : 0.0;
        }

        public List<object> GetTitlesWithRevenue()
        {
            var query = from s in _sessions
                        select new
                        {
                            s.Title,
                            Revenue = s.TicketPrice * s.TicketsSold
                        };
            return query.Cast<object>().ToList();
        }
    }
}