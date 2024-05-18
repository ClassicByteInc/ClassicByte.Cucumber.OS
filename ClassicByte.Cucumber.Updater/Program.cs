using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ClassicByte.Cucumber.Updater
{
    internal class Program
    {
        public static void Main(String[] args)
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
    }
}
