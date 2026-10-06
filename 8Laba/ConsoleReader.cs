using System;

namespace CinemaApp.UI
{
    public class ConsoleReader : IReader
    {
        public string ReadLine(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
    }
}