using System;
using System.IO;
using System.Xml;
using ClassicByte.Cucumber.Core;
using ClassicByte.Cucumber.Core.Exceptions;
using ClassicByte.Cucumber.Core.UserControl;

namespace ClassicByte.Cucumber.App.UserManager
{
    public class Program
    {
        public static int Main(String[] args)
        {
        start: try
            {
                if (args.Length > 0)
                {
                    switch (args[0].ToLower())
                    {
                        case "/login":
                            XmlNodeList usrs;
                            try
                            {
                                if (!File.Exists($"{Core.Path.SystemConfigDir}\\{SystemConfig.USRCFG_NAME}"))
                                {
                                    throw new TypeInitializationException("UserTable", new NullReferenceException());
                                }
                                usrs = SystemConfig.UserTable.Document.GetElementsByTagName("User");
                                var usrList = new String[usrs.Count];
                                if (usrs.Count == 0)
                                {
                                    throw new TypeInitializationException("UserTable", new NullReferenceException());
                                }
                                for (int i = 0; i < usrs.Count; i++)
                                {
                                    usrList[i] = usrs[i].Attributes["USID"].InnerText;

                                }
                                Console.WriteLine("----------------------");
                                Console.WriteLine("此设备上的用户：");
                                foreach (var item in usrList)
                                {
                                    Console.WriteLine(item);
                                }
                                Console.WriteLine("----------------------");
                                Console.Write("输入用户名:");
                                var usrname = Console.ReadLine();
                                Console.Write("输入密码:");
                                var pwd = Console.ReadLine();
                                if (User.Login(usrname, pwd))
                                {
                                    return (int)LoginStatus.SUCCESS;
                                }
                                return 0;
                            }
                            catch (UserException ue)
                            {
                                Console.WriteLine(ue.Message); goto start;

                            }
                            catch (TypeInitializationException)
                            {
                                Console.WriteLine("没有用户,需要注册一个新用户.");
                                Reg();
                                goto start;
                            }
                            catch (Exception ee)
                            {
                                throw new Error(ee.Message, ee.GetType().FullName);
                            }
                    }
                }
                else
                {
                    while (true)
                    {
                        Console.Write("UM>");
                        var command = Console.ReadLine();
                        var cms = command.Split(' ');
                        args = cms;
                        goto start;
                    }
                }
                return (int)LoginStatus.FAILD;
            }
            catch (Error error)
            {
                error.Print();
                throw;
            }
        }

        internal static void Reg()
        {
        l1: Console.Write("输入新的用户名:");
            var usid = Console.ReadLine();
            if ((usid == "") || (usid == null))
            {
                Console.WriteLine("用户名不可为空.");
                goto l1;
            }
        l2: Console.Write($"输入{usid}的密码:");
            var pwd = Console.ReadLine();
            if ((pwd == "") || (pwd == null))
            {
                Console.WriteLine("密码不可为空.");
                goto l2;
            }
            Console.WriteLine($"注册新用户...");
            User.Reg(usid, pwd, UserLevel.OWNER);
        }

    }
    [Flags]
    public enum LoginStatus
    {
        SUCCESS = 0xaa, NOUSER = 0xed, WRONGPASSWORD = 0xdd,
        FAILD = NOUSER | WRONGPASSWORD
    }
}
