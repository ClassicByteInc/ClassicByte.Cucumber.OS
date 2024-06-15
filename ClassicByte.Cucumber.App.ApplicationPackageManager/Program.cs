using System;
using System.IO;
using ClassicByte.Cucumber.Core.Exceptions;
using ClassicByte.Cucumber.Core;
using Console = ClassicByte.Cucumber.Core.Console;

namespace ClassicByte.Cucumber.App.ApplicationPackageManager
{
    internal class Program
    {

        public const string HELPSTR =
    @"此版本可用功能:
    build [Package_Name] [Target_Folder] [outPut] [Install_Location] [Install_Main] [PackageVersion] [PackageDescription]    生成包
    install [Target_Package] [Install_Location]                                        安装包
    init                                                                               关联文件
    list                                                                               列出已安装的包
    uninstall [PackageName]                                                            卸载包
    run [PackageName]                                                                  运行包";


        public static void Main(string[] args)
        {
            Console.WriteLine("ClassicByte App Package Manager 软件包管理 1.0.1");
			try
			{
                if (args.Length == 0)
                {
                    Console.WriteLine(HELPSTR);
                }
                else
                {
                    try
                    {
                        Console.WriteLine($"将要开始的操作:{args[0]}");
                        switch (args[0].ToLower())
                        {
                            case "build":

                                try
                                {

                                    Package.Build(
                                        args[1],
                                        targetDir: new DirectoryInfo(args[2]),
                                        outPut: new DirectoryInfo(args[3]),
                                        appMain: new FileInfo(args[5]), 
                                        args[6], 
                                        args[7]
                                        );
                                }
                                catch (DirectoryNotFoundException e)
                                {
                                    Console.WriteLine(e.Message, ConsoleColor.Red);
                                    Console.WriteLine($"生成失败:{e.Message}", ConsoleColor.Red);
                                    //throw;
                                }
                                catch (FileNotFoundException e)
                                {
                                    Console.WriteLine(e.Message, ConsoleColor.Red);
                                    Console.WriteLine($"生成失败:{e.Message}", ConsoleColor.Red);
                                }
                                break;
                            case "install":
                                try
                                {
                                    new Package(args[1]).Install(args[2]);
                                }
                                catch (FileNotFoundException fnfe)
                                {
                                    Console.WriteLine(fnfe.Message + "安装包损坏。", ConsoleColor.Red);
                                }
                                catch (ArgumentException ae)
                                {
                                    Console.WriteLine(ae.Message, ConsoleColor.Red);
                                }
                                break;
                            case "help":
                                Console.WriteLine(HELPSTR);
                                break;
                            case "init":
                                Package.InitAssembly();
                                break;
                            case "uninstall":
                                Package.Uninstall(args[1]);
                                break;
                            case "list":
                                Console.WriteLine(Package.List());
                                //#errorMessage
                                break;
                            case "download":
                                //#errorMessage
                                break;
                            case "run":
                                break;
                            default:
                                Console.WriteLine($"'{args[0]}'不是命令,键入 apm help 获得帮助", ConsoleColor.Red);
                                break;
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine("命令语法不正确,键入 apm help 获得帮助");
                        //throw;
                    }
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("命令语法不正确。");
            }
            catch (Error e)
            {
                e.Print();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{e.GetType().FullName}]{e.Message}");
#if DEBUG
                throw;
#endif
            }
        }
    }
}
