using System;
using System.Text;

namespace ClassicByte.Cucumber.App.Shell
{
    internal class Program
    {
        public static void Main()
        {

            Console.WriteLine("ClassicByte Cucumber Shell (Managed Windows) v.beta");
            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            while (true)
            {
                Console.Write($"{/*Core.UserControl.User.CurrentUser.USID*/"huang"}${path}>");
                var command = Console.ReadLine();
                ParseCommand(command);
            }
        }

        public static void ParseCommand(String command)
        {
            string[] c = command.Split();
            try
            {
                switch (c[0].ToLower())
                {
                    case "help":
                        Console.WriteLine(GetHelp());
                        break;
                    default:
                        break;
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("命令语法不正确。");
            }
        }
        public static String GetHelp()
        {
            var help = new StringBuilder();
            help.AppendLine("APM        应用程序包管理器");
            help.AppendLine("FM         文件系统管理器");
            help.AppendLine("netsh      托管的网络Shell");
            help.AppendLine("um         用户管理");
            help.AppendLine("help       输出帮助");
            help.AppendLine("CMD        打开Windows的命令提示符");
            help.AppendLine("=========plugins==========");

            return help.ToString();
        }
    }

}
