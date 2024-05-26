using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ClassicByte.Cucumber.Core
{
    public class SystemConfig
    {
        public const String USRCFG_NAME = "2F0F98CD-75EA-48D1-BD62-D05013B04055";
        /// <summary>
        /// 表示配置文件的类
        /// </summary>
        public class Config
        {
            private static FileInfo _fileInfo1;
            /// <summary>
            /// 配置文件的Xml对象
            /// </summary>
            public XmlDocument Document
            {
                get
                {
                    try
                    {
                        var data = ClassicByte.Library.Util.DataEncoder.AESDecryptString(File.ReadAllText(_fileInfo1.FullName), RunTime.USRCFGKEY);
                        var xml = new XmlDocument();
                        xml.LoadXml(data);
                        return xml;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            /// <summary>
            /// 保存配置文件
            /// </summary>
            /// <returns>结果</returns>
            public void Save(XmlDocument xml)
            {
                var data = xml.InnerXml;
                var endata = ClassicByte.Library.Util.DataEncoder.AESEncryptedString(data, RunTime.USRCFGKEY);
                File.WriteAllText(_fileInfo1.FullName, endata);
            }

            /// <summary>
            /// 通过配置文件的文件对象来实例化一个配置
            /// </summary>
            /// <param name="fileInfo"></param>
            internal Config(FileInfo fileInfo)
            {
                _fileInfo1 = fileInfo;
            }


        }
        #region 预定义

        /// <summary>
        /// 用户配置文件对象
        /// </summary>
        /// 
        public static Config UserTable => new Config(new FileInfo($"{ClassicByte.Cucumber.Core.Path.SystemConfigDir.FullName}\\{USRCFG_NAME}"));

        #endregion
    }
}
