using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassicByte.Cucumber.Core.Exceptions;

namespace ClassicByte.Cucumber.App.UserManager
{
    internal class Program
    {
        public static void Main(String[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    switch (args[0].ToLower())
                    {
                        case "/login":
                            //Console.Write("输入用户名：");
                            //var username = Console.ReadLine();
                            //if (ClassicByte.Cucumber.Core.UserControl.User.FindUser(username)==null)
                            //{
                            //    Environment.Exit((Int32)LoginStatus.NOUSER);
                            //}
                            //Console.Write($"输入'{username}'的密码：");
                            //var password = Console.ReadLine();

                            //Environment.Exit((Int32)LoginStatus.SUCCESS);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    while (true)
                    {
                        Console.Write("UM>");
                        var command = Console.ReadLine();
                    }
                }

            }
            catch (Error error)
            {
                Console.WriteLine($"Cucumber 遇到致命错误,现在正在收集信息...\n\n\n错误代码:{error.ErrorCode}\n\n位置:{error.Source}");
            }
        }

    }
    [Flags]
    public enum LoginStatus
    {
        SUCCESS = 0xaa,NOUSER = 0xed,WRONGPASSWORD = 0xdd,
        FAILD = NOUSER| WRONGPASSWORD
    }
}
