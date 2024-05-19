using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Device.Location;

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
            Console.WriteLine("ClassicByte Cucumber Operating System (Managed Microsoft Windows) v.beta");
            Console.WriteLine("Cucumber OS starting...");
            usStk.Push("SYSTEM_SCOPE");
            //检查更新包
            Console.WriteLine("Checking for the update...");

            #region 读取输出
            Process updater = new Process();

            ProcessStartInfo updaterStart = updater.StartInfo;

            updaterStart.CreateNoWindow = true;
            updaterStart.FileName = $"{ClassicByte.Cucumber.Core.Path.SystemCoreDir.FullName}\\sysupdater.exe";
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
            #region 登录
            var um = new Process();
            var umstart = new ProcessStartInfo();
            umstart.FileName = $"{ClassicByte.Cucumber.Core.Path.SystemCoreDir.FullName}\\um.exe";
            umstart.Arguments = "/login";

            um.StartInfo = umstart;
            um.Start();

            um.Exited += (s, e) =>
            {

            };
            um.WaitForExit();
            switch (um.ExitCode)
            {
                case (int)ClassicByte.Cucumber.App.UserManager.LoginStatus.SUCCESS:
                    Console.WriteLine("登录成功");
                    break;
                case (int)App.UserManager.LoginStatus.NOUSER:

                    //MessageBox.Show("没有此用户。", "User Control", MessageBoxButton.OK, MessageBoxImage.Hand);
                    MessageBox.Show("登录到Cucumber失败,原因:\n指定的用户不存在。", "User Control", MessageBoxButton.OK, MessageBoxImage.Hand);
                    //return;
                    break;
                default:
                    break;
            }
            #endregion

            #region 开启cr
            var cr = new Process();
            var crstart = new ProcessStartInfo();
            crstart.FileName = $"{Core.Path.SystemCoreDir}\\csh.exe";

            cr.StartInfo = crstart;
            cr.Start();
            cr.WaitForExit();
            #endregion

        }
    }
}
