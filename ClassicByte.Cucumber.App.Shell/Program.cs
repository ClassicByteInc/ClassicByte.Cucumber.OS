using System;
using System.Text;
using System.Windows;
using ClassicByte.Cucumber.Core.Exceptions;

namespace ClassicByte.Cucumber.App.Shell
{
    internal class Program
    {
        public static void Main()
        {
            try
            {

                Console.WriteLine("ClassicByte Cucumber Shell (Managed Windows) v.beta");
                var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                while (true)
                {
                    try
                    {
                        Console.Write($"{Core.UserControl.User.CurrentUser.USID}${path}>");
                    }
                    catch (UserException)
                    {
                        MessageBox.Show("必须登录Cucumber才能使用.", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
                        return;
                    }
                    var command = Console.ReadLine();
                    ParseCommand(command);
                }
            }
            catch (Error e)
            {
                e.Print();
                throw;
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
