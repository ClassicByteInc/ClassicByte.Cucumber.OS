using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ClassicByte.Cucumber.Core;
using ClassicByte.Library.Util;
using ClassicByte.Library.Util.Zip;
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
                var tempF = $"{Temp}\\apm\\{pkgf.FullName.Replace(pkgf.Extension, "")}";
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

        public void Install()
        {
            //var installDir = pkginfo
        }

        public static void List()
        {

        }

        public static bool Find()
        {
            return false;
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

            var temp = Directory.CreateDirectory($"{Temp}\\{Guid.NewGuid()}\\");
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

                    throw;
                }
            }

            root.AppendChild(installItems);
            root.AppendChild(info);
            installConfigDoc.AppendChild(root);
            installConfigDoc.Save($"{temp}\\{_packagefile}");
            #endregion

            #region 打包
            Directory.CreateDirectory(temp.FullName + "\\Application");
            Directory.CreateDirectory(temp.FullName + "\\Config");

            FileManager.CopyFolder(targetDir.FullName, temp.FullName + "\\Application"); ;
            File.Copy($"{temp}\\{_packagefile}", temp.FullName + "\\Config\\install.xml", true);
            ZipHelper.ZipFileDirectory(temp.FullName, outPut.FullName + $"\\{packageName}.cap", (m) =>
            {
            });
            #endregion

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
