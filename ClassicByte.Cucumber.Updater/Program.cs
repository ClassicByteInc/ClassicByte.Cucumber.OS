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
                Console.WriteLine($"Cucumber 遇到致命错误,现在正在收集信息...\n\n\n错误代码:{error.ErrorCode}\n\n位置:{error.Source}");
            }
        }
    }
}
