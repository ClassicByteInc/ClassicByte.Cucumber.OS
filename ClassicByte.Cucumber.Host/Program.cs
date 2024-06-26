﻿using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Windows;
using ClassicByte.Cucumber.Core;
using ClassicByte.Cucumber.Core.Exceptions;
using ClassicByte.Library.Util;

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
            try
            {
                if (args.Length>0)
                {
                    switch (args[0].ToLower())
                    {
                        case "init":
                            Environment.SetEnvironmentVariable("Path",$"{Environment.GetEnvironmentVariable("Path",EnvironmentVariableTarget.User)};{Core.Path.SystemDir};{Core.Path.SystemCoreDir}",EnvironmentVariableTarget.User);
                            File.WriteAllText($"{Core.Path.SystemConfigDir.FullName}\\{SystemConfig.USRCFG_NAME}", DataEncoder.AESEncryptedString("<UserTable />", "CLASSICBYTE_CUC_USR"));
                            MessageBox.Show("初始化完成.","提示",MessageBoxButton.OK,MessageBoxImage.Information);
                            break;
                        default:
                            break;
                    }
                }
                else
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
                    //var um = new Process();
                    //var umstart = new ProcessStartInfo();
                    //umstart.FileName = $"{ClassicByte.Cucumber.Core.Path.SystemCoreDir.FullName}\\um.exe";
                    //umstart.Arguments = "/login";

                    //um.StartInfo = umstart;
                    //um.Start();

                    //um.Exited += (s, e) =>
                    //{

                    //};
                    //um.WaitForExit();
                    //switch (um.ExitCode)
                    //{
                    //    case (int)ClassicByte.Cucumber.App.UserManager.LoginStatus.SUCCESS:
                    //        Console.WriteLine("登录成功");
                    //        break;
                    //    case (int)App.UserManager.LoginStatus.NOUSER:

                    //        //MessageBox.Show("没有此用户。", "User Control", MessageBoxButton.OK, MessageBoxImage.Hand);
                    //        MessageBox.Show("登录到Cucumber失败,原因:\n指定的用户不存在。", "User Control", MessageBoxButton.OK, MessageBoxImage.Hand);
                    //        //return;
                    //        break;
                    //    default:
                    //        throw new Error("未成功登录", "USER_MANAGER_FAILD");
                    //}
                    ClassicByte.Cucumber.App.UserManager.Program.Main(new string[] { "/login"});
                    #endregion

                    #region 开启cr
                    //var cr = new Process();
                    //var crstart = new ProcessStartInfo();
                    //crstart.FileName = $"{Core.Path.SystemCoreDir}\\csh.exe";

                    //cr.StartInfo = crstart;
                    //cr.Start();
                    //cr.WaitForExit();
                    ClassicByte.Cucumber.App.Shell.Program.Main();
                    #endregion

                }
            }
            catch (Error error)
            {
                error.Print();
            }
            finally
            {
                var usrtabel = Core.SystemConfig.UserTable;
                var usrdoc = usrtabel.Document;
                var root = usrdoc.DocumentElement;
                while (true)
                {
                    try
                    {
                        root.RemoveChild(root.SelectSingleNode("CurrentUser"));
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
                usrdoc.AppendChild(root);
                usrtabel.Save(usrdoc);
            }

            RunTime.Dispose();
        }
    }
}
