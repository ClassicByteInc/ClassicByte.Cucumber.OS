using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte.Cucumber.Host
{
    internal class Program
    {
        static Stack usStk = new Stack();
        [STAThread]
#pragma warning disable
        public static void Main(String[] args)
#pragma warning restore
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("ClassicByte Cucumber Operating System (Managed Microsoft Windows) v.beta");
            Console.WriteLine("Cucumber OS starting...");
            usStk.Push("SYSTEM_SCOPE");
            //检查更新包
            Console.WriteLine("Checking for the update...");

            #region 读取输出
            Process updater = new Process();

            ProcessStartInfo updaterStart = updater.StartInfo;

            updaterStart.CreateNoWindow = true;
            updaterStart.FileName = $"{ClassicByte.Cucumber.Core.Path.SystemDir.FullName}\\sysupdater.exe";
            updaterStart.Arguments = "/check";
            updaterStart.UseShellExecute = false;
            updaterStart.RedirectStandardOutput = true;
            updater.Start();
            {
                using (var reader = updater.StandardOutput)
                {
                    while (!reader.EndOfStream)
                    {
                        Console.Write(reader.ReadLine());
                    }
                }
            }

            
            updater.WaitForExit();
            #endregion
            

            //登录


        }
    }
}
