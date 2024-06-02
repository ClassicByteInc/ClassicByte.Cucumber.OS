using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ClassicByte.Library.Util;
using ClassicByte.Library.Util.Zip;
using ClassicByte.Standard;
using Microsoft.Win32;
using ShellProgressBar;
using static System.Threading.Thread;
using static ClassicByte.Library.Standard.StandardLibrary;

namespace ClassicByte.App.ApplicationPackageManager
{
    /// <summary>
    /// 安装类
    /// </summary>
    public class ApplicationPackageInstaller
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        public static object Name { get => "ClassicByte Application Package Manager 软件包管理"; }

        /// <summary>
        /// 帮助字符串
        /// </summary>
        public static string HelpString =
            @"此版本可用功能:
    build [Package_Name] [Target_Folder] [outPut] [Install_Location] [Install_Main] [PackageVersion] [PackageDescription]    生成.cap安装包
    install [Target_Package] [Install_Location]                                        安装.cap安装包
    assoc                                                                              关联文件.cap
    list                                                                               列出已安装的包
    uninstall [PackageName]                                                            卸载包
    run [PackageName]                                                                  运行包";

        public static FileInfo PackageConfig { get => new FileInfo($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\ClassicByte\\AppPackageManager\\package.cfg"); }

        private readonly static ProgressBarOptions options = new ProgressBarOptions
        {
            ProgressCharacter = '\u2588'
                ,

            ProgressBarOnBottom = true
                ,
            BackgroundColor = ConsoleColor.White
            ,
            ForegroundColor = ConsoleColor.Blue,

        };

        private static int Runner(string[] args)
        {
            Console.WriteLine();
            sPrint($"{Name} 1.0.0.4 ClassicByte Inc.");
            //Ping ping = new Ping();
            //ping.Send("www.github.com",10000,File.ReadAllBytes(@"C:\Users\huang\.ClassicByte\Workspace\C#\ClassicByte.App.USBWatcher\ClassicByte.App.USBWatcher.sln"));
            //ping.PingCompleted += Ping_PingCompleted;
            //sPrint();// sPrint();
            if (args.Length == 0)
            {
                sPrint(HelpString);
            }
            else
            {
                try
                {
                    sPrint($"将要开始的操作:{args[0]}");
                    switch (args[0].ToLower())
                    {
                        case "build":

                            try
                            {
                                BuildInstallPackage(args[1], target: new DirectoryInfo(args[2]), outPut: new DirectoryInfo(args[3]), installLocation: args[4], appMain: new FileInfo(args[5]), args[6], args[7]);
                            }
                            catch (DirectoryNotFoundException e)
                            {
                                sPrint(e.Message, ConsoleColor.Red);
                                sPrint($"生成失败:{e.Message}", ConsoleColor.Red);
                                //throw;
                            }
                            catch (FileNotFoundException e)
                            {
                                sPrint(e.Message, ConsoleColor.Red);
                                sPrint($"生成失败:{e.Message}", ConsoleColor.Red);
                            }
                            break;
                        case "install":
                            try
                            {
                                InstallPack(new FileInfo(args[1]), args[2]);
                            }
                            catch (FileNotFoundException fnfe)
                            {
                                sPrint(fnfe.Message + "安装包损坏。", ConsoleColor.Red);
                            }
                            catch (ArgumentException ae)
                            {
                                sPrint(ae.Message, ConsoleColor.Red);
                            }
                            break;
                        case "help":
                            sPrint(HelpString);
                            break;
                        case "assoc":
                            AssocAssembly();
                            break;
                        case "uninstall":
                            UninstallPackage(args[1]);
                            break;
                        case "list":
                            ListPackages();
                            //#errorMessage
                            break;
                        case "download":
                            //#errorMessage
                            break;
                        case "run":
                            RunPackage(args[1]);
                            break;
                        default:
                            sPrint($"'{args[0]}'不是命令,键入 cspm help 获得帮助", ConsoleColor.Red);
                            break;
                    }
                }
                catch (IndexOutOfRangeException)
                {
                    sPrint("命令语法不正确,键入 cspm help 获得帮助");
                    //throw;
                }
            }
            return 0;
        }

        private static void Ping_PingCompleted(object sender, PingCompletedEventArgs e)
        {
            Console.WriteLine(Encoding.Unicode.GetString(e.Reply.Buffer));
        }

        /// <summary>
        /// 关联安装包(.cap)
        /// </summary>
        public static void AssocAssembly()
        {

            //FileManager.CopyFolder();
            sPrint("开始将文件关联写入注册表...");
            //Registry.ClassesRoot.CreateSubKey();
            //RunCmd($"ftype ClassicByte.Application.Package=\"{Process.GetCurrentProcess().MainModule.FileName}\" install \"%1\" .");
            Registry.ClassesRoot.CreateSubKey("ClassicByte.Application.Package", true).CreateSubKey("Shell", true).CreateSubKey("Open", true).CreateSubKey("Command", true).SetValue("", $"\"{Process.GetCurrentProcess().MainModule.FileName}\" install \"%1\" .");
            sPrint($"写入{Registry.CurrentUser.Name}/ClassicByte.Application.Package");
            sPrint($"写入文件类型...");
            Registry.ClassesRoot.CreateSubKey(".cap", true).SetValue("", "ClassicByte.Application.Package", RegistryValueKind.String);
            sPrint("关联扩展名.cap 到 ClassicByte.Application.Package");
            Registry.ClassesRoot.Close();
            var thisEnvironmentVar = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("Path", thisEnvironmentVar += $";{new FileInfo(Process.GetCurrentProcess().MainModule.FileName).Directory.FullName}", EnvironmentVariableTarget.User);
            sPrint("关联完成!", ConsoleColor.Green);
        }

        private static void Main(string[] args)
        {
            PrintSleepTime = 0;
            try
            {
                var debugS = new string[] { "build", "C:\\Users\\huang\\.ClassicByte\\Temp\\PACK", ".", ".", "C:\\Users\\huang\\.ClassicByte\\Temp\\PACK\\dm.txt" };
                var exit = Runner(args);
            }
            catch (System.Security.SecurityException se)
            {
                sPrint("此操作需要管理员权限,请使用管理员权限重启此应用.", ConsoleColor.Red);
                sPrint(se.Message, ConsoleColor.Red);
            }
            catch (InstallException ie)
            {
                sPrint($"安装失败,原因:\n[{ie.GetType().FullName}]{ie.Message}", ConsoleColor.Red);
            }
            catch (IOException ioe)
            {
                sPrint($"[{ioe.GetType()}]{ioe.Message}");
            }
            catch (System.Exception e)
            {
                Print($"[{e.GetType().FullName}]{e.Message}", ConsoleColor.Red);
                throw;
            }

            try
            {
                Directory.Delete(Project.Temp, true);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// ?
        /// </summary>
        /// <param name="message"></param>
        public delegate void outPut(object message);

        /// <summary>
        /// 安装安装包
        /// </summary>
        /// <param name="targetAppPackage">目标包</param>
        /// <param name="installDirectory">安装目标位置</param>
        /// <param name="out">输出</param>
        /// <exception cref="InstallException"></exception>
        public static void InstallPack(FileInfo targetAppPackage, String installDir, outPut @out)

        {
            var workspace = $"{Project.Temp}\\{Guid.NewGuid()}";
            if (installDir == ".")
            {
                installDir = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\ClassicByte\\{targetAppPackage.Name}";
            }
            var installDirectory = new DirectoryInfo(installDir);
            #region 检查
            if (!targetAppPackage.Exists)
            {
                throw new FileNotFoundException($"未找到文件或者没有权限访问它'{targetAppPackage.FullName}'");
            }

            if (installDirectory is null)
            {
                throw new ArgumentNullException(nameof(installDirectory));
            }
            if (targetAppPackage.Extension != ".cap")
            {
                throw new ArgumentException($"不支持的文件扩展名:{targetAppPackage.Extension}");
            }

            if (targetAppPackage is null)
            {
                throw new ArgumentNullException(nameof(targetAppPackage));
            }

            if (!installDirectory.Exists)
            {
                installDirectory.Create();
            }
            #endregion


            sPrint($"开始安装-'{targetAppPackage.FullName}'...");
            @out($"开始安装-'{targetAppPackage.FullName}'...");
            sPrint($"安装位置:{installDirectory.FullName}");

            #region 解压
            sPrint($"正在解压安装包...");
            @out($"正在解压安装包...");

            //解压安装包到temp目录
            try
            {

                ZipHelper.UnZip(targetAppPackage.FullName, workspace, true);
            }
            catch (IOException ioe)
            {
                sPrint(ioe.Message, ConsoleColor.Red);
                @out(ioe.Message);
                throw ioe;
            }


            sPrint($"解压完成", ConsoleColor.Green);
            @out("解压完成");
            #endregion

            //sPrint("开始复制文件...");

            #region 初始化install.xml

            //初始化xml对象
            XmlDocument installConfigDocument = new XmlDocument();
            //load xml 对象
            installConfigDocument.Load($"{workspace}\\config\\install.xml");
            //获取install.xml root元素
            var installRoot = installConfigDocument.DocumentElement;
            //获取installitem元素
            var installItem = installRoot.SelectSingleNode("InstallItems");
            //获取installintem元素下的所有item元素
            var installFileItems = installItem.SelectNodes("Items");
            //获取这个包包名
            var packageName = installConfigDocument.SelectSingleNode("Installer").Attributes[0].Value;


            #endregion


            #region 检查是否已经安装过


            if (FindPackage(packageName, out installDir))
            {
                throw new InstallException($"包'{packageName}'已经安装过,不能重复安装.");
            };


            #endregion


            #region 校验文件完整性(hash)
            //成功的个数
            int sucessCount = 0;
            //失败的信息
            var errorMessage = new StringBuilder("");
            //新建一个progressbar跟进校验文件完整性
            using (var progressbar = new ProgressBar(installFileItems.Count, "校验文件完整性", options))
            {
                //校验,循环遍历installItems中的item
                for (int i = 0; i < installFileItems.Count; i++)
                {
                    //sPrint(installFileItems[i].InnerText);
                    //新建文件对象,表示installitem中的文件
                    FileInfo fileInfo = new FileInfo($"{workspace}\\Application\\{installFileItems[i].InnerText}");
                    //新建文件夹
                    Directory.CreateDirectory(fileInfo.DirectoryName);
                    //判断installitem中的Hash属性中的hash码是否和文件的hash码一样
                    if (installFileItems[i].Attributes[0].Value == FileManager.GetHash($"{workspace}\\Application\\{installFileItems[i].InnerText}"))
                    {
                        ;
                        //tick进度
                        progressbar.Tick($"文件'{installFileItems[i].InnerText}'已通过哈希码:{installFileItems[i].Attributes[0].Value}");
                        @out($"文件'{installFileItems[i].InnerText}'已通过哈希码:{installFileItems[i].Attributes[0].Value}");
                        //成功次数加一
                        sucessCount++;
                    }
                    else
                    {
                        //tick进度
                        progressbar.Tick($"文件'{installFileItems[i].InnerText}'未通过哈希码:{installFileItems[i].Attributes[0].Value},可能已经损坏.");
                        @out($"文件'{installFileItems[i].InnerText}'未通过哈希码:{installFileItems[i].Attributes[0].Value}");
                        //向错误信息中写入未通过hash码
                        errorMessage.Append($"文件'{installFileItems[i].InnerText}'未通过哈希码:{installFileItems[i].Attributes[0].Value}\n");
                    }
                }
            }
            //如果成功次数不等于文件个数
            if (sucessCount != installFileItems.Count)
            {
                //抛出异常
                throw new InstallException($"未能安装此安装包:文件已损坏:{errorMessage}");
            }
            sPrint("校验文件完成.", ConsoleColor.Green);
            @out("校验文件完成.");


            #endregion


            #region 安装
            //开始安装包
            var installPath = new DirectoryInfo(installDirectory.FullName + "\\" + packageName);
            //创建安装文件夹 
            installPath.Create();
            sPrint("安装目录:" + installPath.FullName);
            @out("安装目录:" + installPath.FullName);
            //新建progressbar跟进复制文件进度
            using (var prgb = new ProgressBar(Directory.GetFiles(workspace + "\\Application\\").Length, "复制文件", options))
            {
                //复制文件夹到安装路径
                FileManager.CopyFolder(workspace + "\\Application\\", installPath.FullName, (m) =>
                {
                    //sPrint(m);
                    prgb.Tick(m.ToString());
                });
            }
            sPrint("安装完成.", ConsoleColor.Green);
            @out("安装完成");
            #endregion


            #region 收尾,创建快捷方式到桌面
            sPrint("创建快捷方式.");
            @out("创建快捷方式.");
            var mainFile = $"{installPath}\\{XDocument.Load($"{workspace}\\config\\install.xml").XPathSelectElement("Installer/Config/Main").Value}";
            //创建快捷方式
            FileManager.CreateDesktopShortcut("", $"{packageName/*目标文件*/}", mainFile);
            sPrint("创建成功.", ConsoleColor.Green);
            @out("创建成功.");
            AppendPackageToConfig(packageName, installDirectory.FullName, installConfigDocument.DocumentElement.Attributes[1].InnerText, installConfigDocument.DocumentElement.Attributes[2].InnerText);
            #endregion

            //安装完成
            sPrint($"安装'{packageName}'成功!");
            @out($"安装'{packageName}'成功!");
            //删除temp文件夹
            try
            {
                Directory.Delete(Project.Temp, true);
            }
            catch (Exception)
            {

                //throw;
            }
        }

        /// <summary>
        /// 安装安装包
        /// </summary>
        /// 
        /// <param name="targetPack">目标包</param>
        /// <param name="installDirectory">安装目标位置</param>
        /// 
        public static void InstallPack(FileInfo targetPack, String installDirectory)
        {
            InstallPack(targetPack, installDirectory, (mess) => { });
        }

        /// <summary>
        /// 生成安装包
        /// </summary>
        /// <param name="ProjectName">项目名称</param>
        /// <param name="outPut">包输出路径</param>
        /// <param name="appMain">应用程序入口</param>
        /// <param name="installLocation">安装位置</param>
        /// <param name="target">目标文件夹</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void BuildInstallPackage(String ProjectName, DirectoryInfo target, DirectoryInfo outPut, string installLocation, FileInfo appMain, String version, String description)
        {
            BuildInstallPackage(ProjectName, target, outPut, installLocation, appMain, (m) => { }, version, description);
        }

        /// <summary>
        /// 生成安装包
        /// </summary>
        /// <param name="packageName">项目名称</param>
        /// <param name="outPut">包输出路径</param>
        /// <param name="appMain">应用程序入口</param>
        /// <param name="installLocation">安装位置</param>
        /// <param name="targetDir">目标文件夹</param>
        /// <exception cref="DirectoryNotFoundException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        public static void BuildInstallPackage(String packageName, DirectoryInfo targetDir,
            DirectoryInfo outPut, string installLocation, FileInfo appMain, outPut print, String version, String description = "")
        {
            #region 检查

            if (targetDir is null)
            {
                throw new ArgumentNullException(nameof(targetDir));
            }

            if (string.IsNullOrEmpty(installLocation))
            {
                //throw new ArgumentException($"“{nameof(installLocation)}”不能为 null 或空。", nameof(installLocation));
            }

            if (appMain is null)
            {
                throw new ArgumentNullException(nameof(appMain));
            }

            if (outPut is null)
            {
                throw new ArgumentNullException(nameof(outPut));
            }
            #endregion


            var installConfigDocumentTempFile = $"{ClassicByte.Standard.Project.Temp}\\{Guid.NewGuid()}.tmp";

            var installConfigDocument = new XmlDocument();

            XmlDeclaration xmlDecl = installConfigDocument.CreateXmlDeclaration("1.0", "UTF-8", null);

            installConfigDocument.AppendChild(xmlDecl);

            #region 初始化install.xml

            //根节点
            var installConfigRoot = installConfigDocument.CreateElement("Installer");

            //安装项目节点
            var installItems = installConfigDocument.CreateElement("InstallItems");

            //安装配置节点
            var installConfig = installConfigDocument.CreateElement("Config");
            //应用程序入口
            var installConfig_Main = installConfigDocument.CreateElement("Main");
            //安装位置
            var installConfig_Location = installConfigDocument.CreateElement("Location");

            //项目名称
            installConfigRoot.SetAttribute("Name", packageName);
            installConfigRoot.SetAttribute("Version", version);
            installConfigRoot.SetAttribute("Description", description);
            #endregion


            sPrint("开始打包...");
            print("开始打包...");


            //打包目标目录
            DirectoryInfo targetFolder = targetDir;
            sPrint($"打包目标目录:'{targetFolder.FullName}'"); ;
            print($"打包目标目录:'{targetFolder}'");
            if (!targetFolder.Exists)
            {
                throw new DirectoryNotFoundException($"'{targetFolder.FullName}':未找到此目录或者没有权限访问此目录");
            }



            //包输出目录
            //var outPut = outPut;
            sPrint($"包输出目录:'{outPut.FullName}'"); ;
            print($"包输出目录:'{outPut.FullName}'");
            if (!outPut.Exists)
            {
                outPut.Create();
            }


            //文件入口
            var mainFile = appMain;
            sPrint($"安装应用程序入口:'{mainFile.Name}'"); ;
            print($"安装应用程序入口:'{mainFile.Name}'");
            if (!mainFile.Exists)
            {
                mainFile = new FileInfo(targetFolder.FullName + "\\" + mainFile.Name);
                if (!mainFile.Exists)
                {
                    throw new InstallException("未能选择应用程序入口.", new FileNotFoundException($"未能找到文件:'{mainFile.FullName}'"));
                }
            }

            installConfig_Main.InnerText = mainFile.Name;

            installConfig.AppendChild(installConfig_Main);

            //安装位置
            var installPath = new FileInfo(installLocation);
            installConfig_Location.InnerText = installPath.Name;
            installConfig.AppendChild(installConfig_Location);

            var fileList = FileManager.FileOut(targetFolder.FullName, (s) => { });


            //var fileList = fileList;
            sPrint($"共有 {fileList.Count} 个目标文件."); ;
            print($"共有 {fileList.Count} 个目标文件.");

            long packFailed = 0;

            String[] fileListHash = new string[fileList.Count];

            int totalTicks = fileList.Count;

            using (var pbar = new ProgressBar(totalTicks, "扫描进度", options))
            {
                #region 扫描循环
                Console.WriteLine("扫描进度:");
                print("扫描进度:");
                for (var j = 0; j < fileList.Count; j++)
                {
                    try
                    {
                        ;
                        fileListHash[j] = FileManager.GetHash(fileList[j].FullName);

                        var Fitem = installConfigDocument.CreateElement("Items");
                        if ((fileList[j].Directory.Name == targetFolder.Name))
                        {
                            Fitem.InnerText = fileList[j].Name;
                        }
                        else
                        {
                            Fitem.InnerText = $"{fileList[j].Directory.FullName.Replace(targetFolder.FullName, "")}\\{fileList[j]}";
                        }

                        var fileHash = installConfigDocument.CreateAttribute("Hash");
                        fileHash.InnerText = fileListHash[j];
                        Fitem.SetAttributeNode(fileHash);
                        installItems.AppendChild(Fitem);

                        print($"正在扫描文件'{fileList[j]}'-剩余{j}/{fileList.Count}--{((double)((double)j / (double)fileList.Count)) * 100}%");
                        pbar.Tick($"扫描完成 '{fileList[j]}'--{fileListHash[j]}");
                        //print(fileListHash[j]);
                    }
                    catch (UnauthorizedAccessException une)
                    {
                        ;
                        fileListHash[j] = "null";
                        pbar.Tick(une.Message);
                        print(une.Message);
                        packFailed++;
                        throw new InstallException($"{une.Message}");
                    }
                    catch (IOException ioe)
                    {
                        ;
                        fileListHash[j] = "null";
                        pbar.Tick(ioe.Message);
                        print(ioe.Message);
                        packFailed++;
                        throw new InstallException($"{ioe.Message}");
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                #endregion
            }

            ;
            //结束扫描
            sPrint($"成功{fileList.Count - packFailed}项,失败{packFailed}项,共{fileList.Count}项");
            print($"成功{fileList.Count - packFailed}项,失败{packFailed}项,共{fileList.Count}项");


            #region 写入配置文件

            installConfigRoot.AppendChild(installItems);
            installConfigRoot.AppendChild(installConfig);
            installConfigDocument.AppendChild(installConfigRoot);
            installConfigDocument.Save(installConfigDocumentTempFile);
            //sPrint(File.ReadAllText(installConfigDocumentTempFile));
            #endregion

            #region 打包
            ;
            sPrint("开始压缩...");
            print("开始压缩...");

            //把要压缩的东西复制到temp:ClassicByte.Standard.Project.Temp + "\\" + Guid.NewGuid().ToString()
            DirectoryInfo temp = new DirectoryInfo($"{Project.Temp}\\{Guid.NewGuid()}");

            temp.Create();

            Directory.CreateDirectory(temp.FullName + "\\Application");
            Directory.CreateDirectory(temp.FullName + "\\Config");

            FileManager.CopyFolder(targetFolder.FullName, temp.FullName + "\\Application");
            File.Copy(installConfigDocumentTempFile, temp.FullName + "\\Config\\install.xml", true);

            //sPrint(outPut.FullName + $"\\{mainFile}.cap");
            //using (var prg = new ProgressBar(fileList.Count, "压缩文件", ConsoleColor.Green))
            //{
            //ZipHelper.FileCount = fileList.Count;
            ZipHelper.ZipFileDirectory(temp.FullName, outPut.FullName + $"\\{packageName}.cap", (m) =>
            {
            });
            //}
            #endregion
            ;
            sPrint("打包完成,位置:" + outPut.FullName + $"\\{packageName}.cap", ConsoleColor.Green);
            print("打包完成,位置:" + outPut.FullName + $"\\{packageName}.cap");
            try
            {
                Directory.Delete(Project.Temp, true);
            }
            catch (Exception)
            {

                //throw;
            }
        }

        /// <summary>
        /// 卸载包
        /// </summary>
        public static void UninstallPackage(String packageName)
        {
            sPrint("启动卸载操作...");
            #region 收集卸载信息

            sPrint($"正在收集有关'{packageName}'包的信息");

            var packageCfg = new XmlDocument();
            packageCfg.Load(PackageConfig.FullName);
            var root = packageCfg.DocumentElement;
            var packs = root.ChildNodes;
            var packsSet = new List<String>();
            foreach (XmlNode item in packs)
            {
                packsSet.Add(item.InnerText);
            }

            if (!packsSet.Contains(packageName))
            {
                sPrint($"找不到包:'{packageName}'", ConsoleColor.Red);
                return;
            }
            var pack = packs[packsSet.IndexOf(packageName)];

            //输出信息
            sPrint($"即将卸载包:{pack.InnerText},位于:{pack.Attributes[0].InnerText}");
            #endregion

            #region 删除目录

            DirectoryInfo delTarget = new DirectoryInfo(pack.Attributes[0].InnerText);
            sPrint($"删除目录:{delTarget.FullName}");
            try
            {
                delTarget.Delete(true);
            }
            catch (Exception e)
            {
                throw new InstallException($"未能卸载包'{pack.InnerText}',{e.Message}", e);
            }
            sPrint($"删除目录'{delTarget.FullName}'成功", ConsoleColor.Red);

            #endregion
            #region 删除配置文件中的项

            sPrint($"正在从package.cfg中移除包'{pack.InnerText}'");
            root.RemoveChild(pack);

            packageCfg.Save(PackageConfig.FullName);
            sPrint("移除成功!", ConsoleColor.Green);

            #endregion
            sPrint($"包'{pack.InnerText}'已经成功被卸载.", ConsoleColor.Green);
        }

        /// <summary>
        /// 将应用程序加入packages.cfg中
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="installPath">安装路径</param>
        public static void AppendPackageToConfig(String packageName, String installPath, String version, String description)
        {

            if (!PackageConfig.Exists)
            {
                XmlDocument packageConfig = new XmlDocument();

                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\ClassicByte\\AppPackageManager\\");

                var configRoot = packageConfig.CreateElement("Packages");

                var packageItemName = packageConfig.CreateElement("Item");

                //var packageItemPath = packageConfig.CreateAttribute("Path");

                packageItemName.InnerText = packageName;

                //packageItemPath.InnerText = installPath;

                packageItemName.SetAttribute("Path", installPath);

                configRoot.AppendChild(packageItemName);

                packageConfig.AppendChild(configRoot);

                packageConfig.Save(PackageConfig.FullName);

                return;
            }
            else
            {
                XmlDocument packageConfigFile = new XmlDocument();

                packageConfigFile.Load(PackageConfig.FullName);

                var root = packageConfigFile.DocumentElement;

                var packageItemName = packageConfigFile.CreateElement("Item");

                //var packageItemPath = packageConfig.CreateAttribute("Path");

                packageItemName.InnerText = packageName;

                //packageItemPath.InnerText = installPath;

                packageItemName.SetAttribute("Path", installPath);
                packageItemName.SetAttribute("Version", version);
                packageItemName.SetAttribute("Description", description);

                root.AppendChild(packageItemName);

                packageConfigFile.AppendChild(root);

                packageConfigFile.Save(PackageConfig.FullName);
            }


        }

        /// <summary>
        /// 输出package.cfg中的包
        /// </summary>
        public static void ListPackages()
        {
            if (!PackageConfig.Exists)
            {
                sPrint("还没有安装任何包.");
                return;
            }
            sPrint($"已安装在用户'{Environment.UserName}'上的包:");
            var packCgf = new XmlDocument();
            packCgf.Load(PackageConfig.FullName);
            var len = packCgf.DocumentElement.ChildNodes.Count;
            var packagesName = new String[len];
            var packagesPath = new FileInfo[len];
            var packagesDes = new String[len];
            var packageVersion = new String[len];
            for (int i = 0; i < len; i++)
            {
                packagesName[i] = packCgf.DocumentElement.ChildNodes[i].InnerText;
                packagesPath[i] = new FileInfo(packCgf.DocumentElement.ChildNodes[i].Attributes[0].InnerText);
                packageVersion[i] = packCgf.DocumentElement.ChildNodes[i].Attributes[1].InnerText;
                packagesDes[i] = packCgf.DocumentElement.ChildNodes[i].Attributes[2].InnerText;
            }
            for (int i = 0; i < len; i++)
            {
                try
                {
                    sPrint($"[{i + 1}]名称:{packagesName[i]},路径:{packagesPath[i].FullName},版本:{packageVersion[i]},描述:'{packagesDes[i]}'");
                }
                catch (NullReferenceException)
                {
                    sPrint("还没有安装任何包.");
                }
                //catch (IndexOutOfRangeException)
                //{

                //}
            }
        }

        /// <summary>
        /// 运行指定包
        /// </summary>
        /// <param name="packageName">要运行的包</param>
        public static void RunPackage(String packageName)
        {
            if (FindPackage(packageName, out string location))
            {
                try
                {
                    Process.Start(location);
                    return;
                }
                catch (Exception e)
                {
                    sPrint(e.Message, ConsoleColor.Red);
                    return;
                }
            }
            else
            {
                sPrint($"找不到包:'{packageName}'");
            }
        }

        /// <summary>
        /// 在package.cfg中查找包名
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="location"></param>
        /// <returns>如果存在,返回 true 和 包的位置 ;如果不存在,返回 false </returns>
        public static bool FindPackage(String packageName, out String location)
        {
            try
            {
                var packageCfg = new XmlDocument();

                packageCfg.Load(PackageConfig.FullName);

                var eroot = packageCfg.DocumentElement;

                var packs = eroot.ChildNodes;

                var packsSet = new List<String>();

                foreach (XmlNode item in packs)

                {

                    packsSet.Add(item.InnerText);
                }




                if (packsSet.Contains(packageName))

                {

                    location = packs[packsSet.IndexOf(packageName)].Attributes[0].InnerText;

                    return true;
                }


                location = null;

                return false;
            }


            catch (Exception)

            {

                location = null;

                return false;
            }

        }
    }
    /// <summary>
    /// 安装时所引发的异常
    /// </summary>
    [Serializable]

    public class InstallException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        public InstallException() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public InstallException(string message) : base(message) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public InstallException(string message, Exception inner) : base(message, inner) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected InstallException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}