using System;

namespace ClassicByte.Cucumber.Updater
{
    internal class Program
    {
        public static void Main(String[] args)
        {
            try
            {
                switch (args[0].ToLower())
                {
                    case "/check":
                        Console.WriteLine("Search for the update package files...");
                        break;
                    default:
                        break;
                }
            }
            catch (ClassicByte.Cucumber.Core.Exceptions.Error error)
            {
                error.Print(); throw;
            }
        }
    }
}
