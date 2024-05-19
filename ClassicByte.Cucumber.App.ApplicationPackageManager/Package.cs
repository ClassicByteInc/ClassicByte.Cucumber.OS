using System;
using System.IO;
using System.Xml;
using ClassicByte.Library.Util.Zip;
using static ClassicByte.Cucumber.Core.Path;

namespace ClassicByte.Cucumber.App.ApplicationPackageManager
{
    public class Package
    {
        public const String PKGINFONAME = "__PKGINFO__.__INFO";
        public static FileInfo PackageInfoFile => new FileInfo($"{ClassicByte.Cucumber.Core.Path.SystemConfig.FullName}\\pkgs.cfg");
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

                throw new InstallException("包已损坏。",nre);
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
