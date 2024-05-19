using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte.Cucumber.App.Shell
{
    internal class Program
    {
        public static void Main()
        {

            Console.WriteLine("ClassicByte Cucumber Shell (Managed Windows) v.beta");
            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            while (true) {
                Console.Write($"{/*Core.UserControl.User.CurrentUser.USID*/"huang"}${path}>");
                var command = Console.ReadLine();
            }
        }

     public static void ParseCommand(String command){
         string[] c = command.Split();
         try{
         
         }catch(IndexOutOfRangeException){
         Console.WriteLine("命令语法不正确。");}
        }
    }
    public static String GetHelp()
}
