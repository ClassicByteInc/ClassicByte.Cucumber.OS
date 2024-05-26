using System;

namespace ClassicByte.Cucumber.App.NetShell
{
    internal class Program
    {
        public static void Main()
        {
            var sth = Console.ReadLine();
            Console.WriteLine(ClassicByte.Library.Util.DataEncoder.AESEncryptedString(sth, "CLASSICBYTE_CUC_USR"));
            Console.ReadKey();
        }
    }
}
