using System;
using System.Collections.Generic;
using System.IO;
using CinemaApp.Models;

namespace CinemaApp.Services
{
    public class BinarySessionStorage : ISessionStorage
    {
        private readonly string _filePath;

        public BinarySessionStorage(string filePath)
        {
            _filePath = filePath;
        }

        public List<MovieSession> Load()
        {
            var result = new List<MovieSession>();
            if (!File.Exists(_filePath)) return result;

            using var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new BinaryReader(stream);

            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var session = new MovieSession
                {
                    Id = reader.ReadInt32(),
                    Title = reader.ReadString(),
                    Genre = reader.ReadString(),
                    StartTime = DateTime.FromBinary(reader.ReadInt64()),
                    DurationMinutes = reader.ReadInt32(),
                    TicketPrice = reader.ReadDecimal(),
                    TicketsSold = reader.ReadInt32()
                };
                result.Add(session);
            }
            return result;
        }

        public void Save(List<MovieSession> sessions)
        {
            using var stream = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var writer = new BinaryWriter(stream);

            writer.Write(sessions.Count);
            foreach (var s in sessions)
            {
                writer.Write(s.Id);
                writer.Write(s.Title);
                writer.Write(s.Genre);
                writer.Write(s.StartTime.ToBinary());
                writer.Write(s.DurationMinutes);
                writer.Write(s.TicketPrice);
                writer.Write(s.TicketsSold);
            }
        }
    }
}