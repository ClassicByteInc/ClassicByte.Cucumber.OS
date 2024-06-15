using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ClassicByte.Cucumber.Core;
using ClassicByte.Library.Util;
using ClassicByte.Library.Util.Zip;
using ClassicByte.Standard;
using Microsoft.Win32;
using static ClassicByte.Cucumber.Core.Path;
using Console = ClassicByte.Cucumber.Core.Console;

namespace ClassicByte.Cucumber.App.ApplicationPackageManager
{
    public class Package
    {
        public const String PackageExtension = @".AP";
        public const String PKGINFONAME = @"__PKGINFO__.__INFO";
        public static FileInfo PackageInfoFile => new FileInfo($"{ClassicByte.Cucumber.Core.Path.SystemConfigDir.FullName}\\pkgs.cfg");
        public static XmlDocument PackageInfo
        {
            get
            {
                var xml = new XmlDocument();
                if (!PackageInfoFile.Exists)
                {
                    PackageInfoFile.Create();
                    xml.Load(PackageInfoFile.FullName);
                    var root = xml.CreateElement("Packages");
                    root.SetAttribute("Version", "std1.0");
                    xml.AppendChild(root);
                    xml.Save(PackageInfoFile.FullName);
                    return xml;
                }
                xml.Load(PackageInfoFile.FullName);
                return xml;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private XmlDocument pkginfo { get; set; }
        /// <summary>
        /// 
        /// </summary>
        private DirectoryInfo temp { get; set; }

        /// <summary>
        /// 包的名称
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 包的描述
        /// </summary>
        public String Description { get; set; }
        /// <summary>
        /// 包的版本
        /// </summary>
        public String Version { get; set; }

        /// <summary>
        /// 包的作者
        /// </summary>
        public String Author { get; set; }

        /// <summary>
        /// 包的文件对象
        /// </summary>
        public FileInfo FileInfo { get; set; }

        /// <summary>
        /// 包的ID
        /// </summary>
        public String ID { get; set; }

        public Package(String path)
        {
            try
            {
                Console.WriteLine($"开始实例化包：'{path}'");
                this.FileInfo = new FileInfo(path);
                Console.WriteLine($"包位置：'{FileInfo.FullName}'");
                var pkgf = new FileInfo(path);
                var tempF = $"{Temp.FullName}\\apm\\{pkgf.Name.Replace(pkgf.Extension, "")}";
                temp = new DirectoryInfo(tempF);
                #region 解压到Temp
                Console.WriteLine("解压包中...");
                ZipHelper.UnZip(pkgf.FullName, tempF);
                Console.WriteLine("解压成功！",ConsoleColor.Green);
                #endregion

                #region 读取pkginfo
                var pkginfoxml = new XmlDocument();
                pkginfoxml.Load($"{tempF}\\config\\{PKGINFONAME}");
                System.Console.WriteLine("开始初始化包配置信息...");
                pkginfo = pkginfoxml;
                var inforoot = pkginfoxml.DocumentElement.GetElementsByTagName("PackageInfo")[0];
                this.ID = inforoot.Attributes["Name"].Value;
                Console.WriteLine($"包ID:{ID}");
                //this.Author = inforoot.Attributes["Author"].Value;
                this.Version = inforoot.Attributes["Version"].Value;
                Console.WriteLine($"包版本:{Version}");
                this.Name = inforoot.Attributes["Name"].Value;
                this.Description = inforoot.Attributes["Description"].Value;
                Console.WriteLine($"包描述:{Description}");
                #endregion
            }
            catch (NullReferenceException nre)
            {

                throw new InstallException("包已损坏。", nre);
            }
        }

        /// <summary>
        /// 安装包
        /// </summary>
        /// <param name="installDir"></param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InstallException"></exception>
        public void Install(String installDir)

        {
            Console.WriteLine($"开始安装包'{Name}'");
            var targetAppPackage = this.FileInfo;
            var workspace = $"{Temp.FullName}\\{Guid.NewGuid()}";
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
            if (targetAppPackage.Extension != PackageExtension)
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


            #region 解压

            //解压安装包到temp目录
            try
            {
                Console.WriteLine($"解压包'{Name}'->'{workspace}'");
                ZipHelper.UnZip(targetAppPackage.FullName, workspace, true);
                System.Console.WriteLine("解压完成.");
            }
            catch (IOException ioe)
            {
                throw ioe;
            }
            #endregion

            //sPrint("开始复制文件...");

            #region 初始化install.xml

            //初始化xml对象
            XmlDocument installConfigDocument = new XmlDocument();
            Console.WriteLine("初始化配置文件...");
            //load xml 对象
            installConfigDocument.Load($"{workspace}\\config\\{PKGINFONAME}");
            //获取install.xml root元素
            var installRoot = installConfigDocument.DocumentElement;
            //获取installitem元素
            var installItem = installRoot.SelectSingleNode("InstallItems");
            //获取installintem元素下的所有item元素
            var installFileItems = installItem.SelectNodes("Items");
            //获取这个包包名
            var packageName = Name;


            #endregion


            #region 检查是否已经安装过


            if (Find(packageName, out installDir))
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
            //校验,循环遍历installItems中的item
            System.Console.WriteLine("校验文件完整性...");
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
                    //成功次数加一
                    Console.WriteLine($"文件'{installFileItems[i].InnerText}'已通过哈希:'{installFileItems[i].Attributes[0].Value}'");
                    sucessCount++;
                }
                else
                {
                    //tick进度
                    //向错误信息中写入未通过hash码
                    errorMessage.Append($"文件'{installFileItems[i].InnerText}'未通过哈希码:{installFileItems[i].Attributes[0].Value}\n");
                }
            }
            //如果成功次数不等于文件个数
            if (sucessCount != installFileItems.Count)
            {
                //抛出异常
                throw new InstallException($"未能安装此安装包:文件已损坏:{errorMessage}");
            }


            #endregion


            #region 安装
            //开始安装包
            var installPath = new DirectoryInfo(installDirectory.FullName + "\\" + packageName);
            //创建安装文件夹 
            installPath.Create();
            FileManager.CopyFolder(workspace + "\\Application\\", installPath.FullName, (m) =>
            {
            });
            #endregion


            #region 收尾,创建快捷方式到桌面
            var mainFile = $"{installPath}\\{XDocument.Load($"{workspace}\\config\\install.xml").XPathSelectElement("Installer/Config/Main").Value}";
            //创建快捷方式
            FileManager.CreateDesktopShortcut("", $"{packageName/*目标文件*/}", mainFile);
            AppendPackageToConfig(packageName, installDirectory.FullName, installConfigDocument.DocumentElement.Attributes[1].InnerText, installConfigDocument.DocumentElement.Attributes[2].InnerText);
            #endregion

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
        /// 输出package.cfg中的包
        /// </summary>
        public static String List()
        {
            var output = new StringBuilder();
            if (!SystemConfig.PackageTable.FileInfo.Exists)
            {
                throw new Core.Exceptions.CoreException("配置文件不存在。");
            }
            output.AppendLine($"已安装在用户'{Environment.UserName}'上的包:");
            var packCgf = new XmlDocument();
            packCgf.Load(SystemConfig.PackageTable.FileInfo.FullName);
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
                    output.AppendLine($"[{i + 1}]名称:{packagesName[i]},路径:{packagesPath[i].FullName},版本:{packageVersion[i]},描述:'{packagesDes[i]}'");
                }
                catch (NullReferenceException)
                {
                    output.AppendLine("还没有安装任何包.");
                }
                //catch (IndexOutOfRangeException)
                //{

                //}
            }
            return output.ToString();
        }

        public static bool Find(String packageName, out String location)
        {
            try
            {
                var packageCfg = new XmlDocument();

                packageCfg.Load(SystemConfig.PackageTable.Document.InnerXml);

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

        /// <summary>
        /// 卸载包
        /// </summary>
        public static void Uninstall(String packageName)
        {
            var packageCfg = new XmlDocument();
            packageCfg.Load(SystemConfig.PackageTable.FileInfo.FullName);
            var root = packageCfg.DocumentElement;
            var packs = root.ChildNodes;
            var packsSet = new List<String>();
            foreach (XmlNode item in packs)
            {
                packsSet.Add(item.InnerText);
            }

            if (!packsSet.Contains(packageName))
            {
                return;
            }
            var pack = packs[packsSet.IndexOf(packageName)];

            //输出信息
            #region 删除目录

            DirectoryInfo delTarget = new DirectoryInfo(pack.Attributes[0].InnerText);
            try
            {
                delTarget.Delete(true);
            }
            catch (Exception e)
            {
                throw new InstallException($"未能卸载包'{pack.InnerText}',{e.Message}", e);
            }

            #endregion
            #region 删除配置文件中的项

            root.RemoveChild(pack);

            packageCfg.Save(SystemConfig.PackageTable.FileInfo.FullName);

            #endregion
        }

        /// <summary>
        /// 生成包,返回包对象
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="targetDir"></param>
        /// <param name="outPut"></param>
        /// <param name="installLocation"></param>
        /// <param name="appMain"></param>
        /// <param name="version"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public static Package Build(
            String packageName, //包名称
            DirectoryInfo targetDir, //打包的目标文件夹
            DirectoryInfo outPut, //输出文件夹
            FileInfo appMain, //包的入口
            String version,
            String description = "")
        {

            #region 检查
            if (string.IsNullOrEmpty(packageName))
            {
                throw new ArgumentException($"“{nameof(packageName)}”不能为 null 或空。", nameof(packageName));
            }

            if (targetDir is null)
            {
                throw new ArgumentNullException(nameof(targetDir));
            }

            if (outPut is null)
            {
                throw new ArgumentNullException(nameof(outPut));
            }


            if (appMain is null)
            {
                throw new ArgumentNullException(nameof(appMain));
            }

            if (string.IsNullOrEmpty(version))
            {
                throw new ArgumentException($"“{nameof(version)}”不能为 null 或空。", nameof(version));
            }

            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException($"“{nameof(description)}”不能为 null 或空。", nameof(description));
            }

            if (!targetDir.Exists)
            {
                throw new DirectoryNotFoundException($"'{targetDir.FullName}':未找到此目录或者没有权限访问此目录");
            }
            if (!outPut.Exists)
            {
                outPut.Create();
            }
            var mainFile = appMain;
            if (!mainFile.Exists)
            {
                mainFile = new FileInfo(targetDir.FullName + "\\" + mainFile.Name);
                if (!mainFile.Exists)
                {
                    throw new InstallException("未能选择应用程序入口.", new FileNotFoundException($"未能找到文件:'{mainFile.FullName}'"));
                }
            }
            #endregion

            #region 初始化安装包配置

            var installConfigDoc = new XmlDocument();
            var root = installConfigDoc.CreateElement("InstallConfig");
            root.SetAttribute("Version", "std");
            var info = installConfigDoc.CreateElement("PackageInfo");
            info.SetAttribute("Version", version);
            info.SetAttribute("Name", packageName);
            info.SetAttribute("Description", description);
            var installItems = installConfigDoc.CreateElement("InstallItems");
            root.AppendChild(info);
            #endregion
            var n = $"{Temp.FullName}\\{Guid.NewGuid()}\\";
            Directory.CreateDirectory(n);
            var temp = new DirectoryInfo(n);
            var _packagefile = $"{packageName}{PackageExtension}";

            #region 扫描文件到列表

            var fileList = GetFileNames(targetDir.FullName);
            String[] fileListHash = new string[fileList.Count];
            for (var j = 0; j < fileList.Count; j++)
            {
                try
                {
                    fileListHash[j] = FileManager.GetHash(fileList[j].FullName);

                    var Fitem = installConfigDoc.CreateElement("Items");
                    if ((fileList[j].Directory.Name == targetDir.Name))
                    {
                        Fitem.InnerText = fileList[j].Name;
                    }
                    else
                    {
                        Fitem.InnerText = $"{fileList[j].Directory.FullName.Replace(targetDir.FullName, "")}\\{fileList[j]}";
                    }

                    var fileHash = installConfigDoc.CreateAttribute("Hash");
                    fileHash.InnerText = fileListHash[j];
                    Fitem.SetAttributeNode(fileHash);
                    installItems.AppendChild(Fitem);

                    //print(fileListHash[j]);
                }
                catch (UnauthorizedAccessException une)
                {
                    ;
                    fileListHash[j] = "null";
                    throw new InstallException($"{une.Message}");
                }
                catch (IOException ioe)
                {
                    ;
                    fileListHash[j] = "null";
                    throw new InstallException($"{ioe.Message}");
                }
                catch (Exception)
                {

                }
            }

            root.AppendChild(installItems);
            root.AppendChild(info);
            installConfigDoc.AppendChild(root);
            var work = $"{Temp.FullName}\\{Guid.NewGuid()}";
            var cfgfile = $"{work}\\{PKGINFONAME}";
            Directory.CreateDirectory(work);
            installConfigDoc.Save(cfgfile);
            #endregion

            #region 打包
            Directory.CreateDirectory(temp.FullName + "\\Application");
            Directory.CreateDirectory(temp.FullName + "\\Config");

            FileManager.CopyFolder(targetDir.FullName, temp.FullName + "\\Application"); ;
            File.Copy(cfgfile, temp.FullName + $"\\Config\\{PKGINFONAME}", true);
            ZipHelper.ZipFileDirectory(temp.FullName, outPut.FullName + $"\\{_packagefile}", (m) =>
            {
            });
            #endregion
            Debug.WriteLine($"{outPut.FullName}\\{_packagefile}");
            return new Package($"{outPut.FullName}\\{_packagefile}");
        }


        static List<FileInfo> GetFileNames(string rootDir)
        {
            List<FileInfo> fileNames = new List<FileInfo>();
            string[] files = Directory.GetFiles(rootDir); // 获取指定文件夹下的所有文件
            foreach (string file in files)
            {
                fileNames.Add(new FileInfo(file)); // 将文件添加到List中
            }
            string[] subDirs = Directory.GetDirectories(rootDir); // 获取指定文件夹下的所有子文件夹
            foreach (string subDir in subDirs)
            {
                fileNames.AddRange(GetFileNames(subDir)); // 递归获取子文件夹下的所有文件
            }
            return fileNames;
        }
        /// <summary>
        /// 将应用程序加入packages.cfg中
        /// </summary>
        /// <param name="packageName"></param>
        /// <param name="installPath">安装路径</param>
        public static void AppendPackageToConfig(String packageName, String installPath, String version, String description)
        {

            if (!SystemConfig.PackageTable.FileInfo.Exists)
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

                SystemConfig.PackageTable.Save(packageConfig);

                return;
            }
            else
            {
                XmlDocument packageConfigFile = new XmlDocument();

                packageConfigFile.Load(SystemConfig.PackageTable.FileInfo.FullName);

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

                SystemConfig.PackageTable.Save(packageConfigFile);
            }


        }
        /// <summary>
        /// 关联安装包(.cap)
        /// </summary>
        public static void InitAssembly()
        {

            //FileManager.CopyFolder();
            //Registry.ClassesRoot.CreateSubKey();
            //RunCmd($"ftype ClassicByte.Application.Package=\"{Process.GetCurrentProcess().MainModule.FileName}\" install \"%1\" .");
            Registry.ClassesRoot.CreateSubKey("ClassicByte.Application.Package", true).CreateSubKey("Shell", true).CreateSubKey("Open", true).CreateSubKey("Command", true).SetValue("", $"\"{Process.GetCurrentProcess().MainModule.FileName}\" install \"%1\" .");
            Registry.ClassesRoot.CreateSubKey(PackageExtension, true).SetValue("", "ClassicByte.Application.Package", RegistryValueKind.String);
            Registry.ClassesRoot.Close();
            var thisEnvironmentVar = Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable("Path", thisEnvironmentVar += $";{new FileInfo(Process.GetCurrentProcess().MainModule.FileName).Directory.FullName}", EnvironmentVariableTarget.User);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class InstallException : Exception
    {
        public InstallException() { }
        public InstallException(string message) : base(message) { }
        public InstallException(string message, Exception inner) : base(message, inner) { }
        protected InstallException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
