using System;
using ClassicByte.Cucumber.Core.Exceptions;

namespace ClassicByte.Cucumber.App.FileSystemManager
{
    internal class Program
    {
        public static void Main(String[] args)
        {
			try
			{
                switch (args[0].ToLower())
                {
                    case "-jiemi":
                        while (true)
                        {
                            try
                            {
                                var sth = Console.ReadLine();
                                Console.WriteLine(ClassicByte.Library.Util.DataEncoder.AESDecryptString(sth, "CLASSICBYTE_CUC_USR"));
                            }
                            catch (Exception)
                            {
                            }
                        }
                        break;
                    case "-jiami":
                        while (true)
                        {
                            try
                            {
                                var sth = Console.ReadLine();
                                Console.WriteLine(ClassicByte.Library.Util.DataEncoder.AESEncryptedString(sth, "CLASSICBYTE_CUC_USR"));
                            }
                            catch (Exception)
                            {


                            }
                        }
                        break;
                    default:
                        break;
                }
            }
			catch (Error e)
			{
				e.Print();
				throw;
			}
        }
    }
}
