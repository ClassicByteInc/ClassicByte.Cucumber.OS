using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

///<remarks></remarks>
namespace ClassicByte.Cucumber.Core.UserControl
{
    /// <summary>
    /// Cucumber 用户对象，提供操作用户的方法
    /// </summary>
    public class User
    {

        #region 私有变量

        private static XmlDocument userConfig { get
            {
                var xml = new XmlDocument();
                try
                {
                    xml.Load($"{Path.SystemConfig.FullName}\\usrtabel.cfg");
                    return xml;
                }
                catch (FileNotFoundException)
                {
                    var users = xml.CreateElement("Users");
                    xml.AppendChild(users);
                    xml.Save($"{Path.SystemConfig.FullName}\\usrtabel.cfg");
                    return xml;
                }
            } }

        #endregion

        public String Name { get; private set; }

        public String USID { get; private set; }

        public UserLevel Level { get; private set; }

        public User(String name, String usid)
        {

        }

        public static User CurrentUser
        {
            get
            {

                return null;
            }
        }
    }

    /// <summary>
    /// 用户权限级别的枚举
    /// </summary>
    public enum UserLevel
    {
        /// <summary>
        /// 安装者级别
        /// </summary>
        INSTALL = 0xccb,

        /// <summary>
        /// 计算机所有者级别
        /// </summary>
        OWNER = 0xfc,

        /// <summary>
        /// 用户级别
        /// </summary>
        USER = 0xaa,

        /// <summary>
        /// 游客级别
        /// </summary>
        GUEST = 0x01
    }
}
