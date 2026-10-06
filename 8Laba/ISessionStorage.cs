using System.Collections.Generic;
using CinemaApp.Models;

namespace CinemaApp.Services
{
    public interface ISessionStorage
    {
        List<MovieSession> Load();
        void Save(List<MovieSession> sessions);
    }
}