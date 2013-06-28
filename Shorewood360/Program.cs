using System;

namespace IETGames.Shorewood
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Shorewood shorewood = new Shorewood())
            {
                shorewood.Run();
            }
        }
    }
}
