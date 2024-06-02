using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using ClassicByte.Cucumber.Core;
using ClassicByte.Library.Util;
using ClassicByte.Library.Util.Zip;
using ClassicByte.Standard;
using static ClassicByte.Cucumber.Core.Path;

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

        private XmlDocument pkginfo { get; set; }
        private DirectoryInfo temp { get; set; }

        public String Name { get; set; }

        public String Description { get; set; }

        public String Version { get; set; }

        public String Author { get; set; }

        public FileInfo FileInfo { get; set; }

        public String ID { get; set; }

        public Package(String path)
        {
            try
            {
                var pkgf = new FileInfo(path);
                var tempF = $"{Temp.FullName}\\apm\\{pkgf.Name.Replace(pkgf.Extension, "")}";
                temp = new DirectoryInfo(tempF);
                #region 解压到Temp
                ZipHelper.UnZip(pkgf.FullName, tempF);
                #endregion

                #region 读取pkginfo
                var pkginfoxml = new XmlDocument();
                pkginfoxml.Load($"{tempF}\\config\\{PKGINFONAME}");
                pkginfo = pkginfoxml;
                var inforoot = pkginfoxml.DocumentElement;
                this.ID = inforoot.GetAttribute("ID");
                this.Author = inforoot.GetAttribute("Author");
                this.Version = inforoot.GetAttribute("Version");
                this.Name = inforoot.GetAttribute("Name");
                #endregion
            }
            catch (NullReferenceException nre)
            {

                throw new InstallException("包已损坏。", nre);
            }
        }

        public void Install(FileInfo targetAppPackage, String installDir)

        {
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

                ZipHelper.UnZip(targetAppPackage.FullName, workspace, true);
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
            //load xml 对象
            installConfigDocument.Load($"{workspace}\\config\\{PKGINFONAME}");
            //获取install.xml root元素
            var installRoot = installConfigDocument.DocumentElement;
            //获取installitem元素
            var installItem = installRoot.SelectSingleNode("InstallItems");
            //获取installintem元素下的所有item元素
            var installFileItems = installItem.SelectNodes("Items");
            //获取这个包包名
            var packageName = installConfigDocument.SelectSingleNode("PackageInfo").Attributes["Name"].Value;


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

        public static void List()
        {

        }

        public static bool Find()
        {
            try
            {
                var packageCfg = new XmlDocument();

                packageCfg.Load(SystemConfig.);

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

        public static void Uninstall()
        {

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
            installConfigDoc.Save($"{temp}\\{PKGINFONAME}");
            #endregion

            #region 打包
            Directory.CreateDirectory(temp.FullName + "\\Application");
            Directory.CreateDirectory(temp.FullName + "\\Config");

            FileManager.CopyFolder(targetDir.FullName, temp.FullName + "\\Application"); ;
            File.Copy($"{temp}\\{PKGINFONAME}", temp.FullName + $"\\Config\\{PKGINFONAME}", true);
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
