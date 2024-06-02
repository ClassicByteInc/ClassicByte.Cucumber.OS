using System;
using ClassicByte.Cucumber.Core.Exceptions;

namespace ClassicByte.Cucumber.App.ApplicationPackageManager
{
    internal class Program
    {

        public const String HELPSTR = @"
install [package_path] 安装指定包
uninstall [package_id] 卸载包
list    列出已经安装的包
init    关联包安装程序
";


        public static void Main(string[] args)
        {
			try
			{
                if (args.Length==0)
                {
                    Console.WriteLine($"\n\nClassicByte Cucumber Package Manager 1.0.0.1\n{HELPSTR}");
                    return;
                }
                switch (args[0].ToLower())
                {
                    case"build":
                        Package.Build(args[1], new System.IO.DirectoryInfo(args[2]),new System.IO.DirectoryInfo(args[3]),new System.IO.FileInfo(args[4]), args[5], args[6]);   
                        break;
                    case "install":
                        new Package(args[1]).Install();
                        break;
                    
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
            catch (Exception)
            {
                throw new Error();
            }
        }
    }
}
