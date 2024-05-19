using System;
using System.IO;
using System.Xml;
using static ClassicByte.Cucumber.Core.Path;

namespace ClassicByte.Cucumber.App.ApplicationPackageManager
{
    public class Package
    {
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
                    root.SetAttribute("Version" ,"std1.0");
                    xml.AppendChild(root);
                    xml.Save(PackageInfoFile.FullName);
                    return xml;
                }
                xml.Load(PackageInfoFile.FullName);
                return xml;
            }
        }

        public String Name { get; set; }

        public String Description { get; set; }

        public String Version { get; set; }

        public String Author { get; set; }

        public FileInfo FileInfo { get; set; }

        public String ID { get; set; }

        public Package(String path)
        {
            #region 解压到Temp

            #endregion
        }

        public static void Install()
        {

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
}
