using System;

namespace CinemaApp.Models
{
    [Serializable]
    public class MovieSession
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Genre { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public decimal TicketPrice { get; set; }
        public int TicketsSold { get; set; }

        public MovieSession() { }

        public MovieSession(int id, string title, string genre,
            DateTime startTime, int durationMinutes,
            decimal ticketPrice, int ticketsSold)
        {
            Id = id;
            Title = title;
            Genre = genre;
            StartTime = startTime;
            DurationMinutes = durationMinutes;
            TicketPrice = ticketPrice;
            TicketsSold = ticketsSold;
        }

        public override string ToString()
        {
            return $"#{Id,-3} | {Title,-25} | {Genre,-12} | " +
                   $"{StartTime:dd.MM.yyyy HH:mm} | {DurationMinutes,3} мин | " +
                   $"{TicketPrice,8:F2} руб. | продано: {TicketsSold}";
        }
    }
}