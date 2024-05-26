using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using ClassicByte.Cucumber.Core.Exceptions;

///<remarks></remarks>
namespace ClassicByte.Cucumber.Core.UserControl
{
    /// <summary>
    /// Cucumber 用户对象，提供操作用户的方法
    /// </summary>
    public class User
    {

        /// <summary>
        /// 用户名称
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// 用户的全局唯一标识符
        /// </summary>
        public String USID { get; private set; }

        /// <summary>
        /// 用户的等级
        /// </summary>
        public UserLevel Level { get; private set; }

        public String Password { get; private set; }

        internal User(String usid, string name, UserLevel userLevel, String pwd)
        {
            USID = usid;
            Name = name;
            Level = userLevel;
            Password = pwd;
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        public static User CurrentUser
        {
            get
            {
                try
                
                {
                    
                    var xml = SystemConfig.UserTable;
                    var xmld = XDocument.Parse(xml.Document.InnerXml);
                    try
                    {
                        var usid = xmld.XPathSelectElement("UserTable/CurrentUser").Attribute("USID").Value;
                        return FindUser(usid);

                    }
                    catch (NullReferenceException)
                    {
                        throw new UserException("当前没有登录用户.") ;
                    }                    
                    
                }
                catch (NullReferenceException)
                {
                    return null;
                }
                catch (UserException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new Error(e.Message, e.Message);
                }
            }
        }

        public static User FindUser(string usid)
        {
            var usrs = SystemConfig.UserTable.Document.GetElementsByTagName("User");
            var usrList = new String[usrs.Count];
            for (int i = 0; i < usrs.Count; i++)
            {
                usrList[i] = usrs[i].Attributes["USID"].Value;
            }
            var index = Array.IndexOf(usrList, usid);
            var target = usrs[index];
            String usrid;
            try
            {
                usrid = target.Attributes["USID"].Value;
            }
            catch (NullReferenceException nre)
            {
                throw new UserException("没有此用户",nre);
            }
            var pwd = target.Attributes["Password"].Value;
            UserLevel userLevel;
            switch (target.Attributes["Level"].Value)
            {
                case "USER":
                    userLevel = UserLevel.USER; break;
                case "OWNER":
                    userLevel = UserLevel.OWNER; break;
                default:
                    throw new CoreException($"未知的级别：{target.Attributes["LEVEL"].Value}");
            }
            return new User(usid, usid, userLevel, pwd);
        }

        public static bool Login(String usid, String pwd)
        {
            //找到用户
            User usr;
            try
            {
                 usr = User.FindUser(usid);
            }
            catch (UserException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Error(e.Message,e);
            }

            if (pwd != usr.Password)
            {
                throw new LogonFailureException("密码错误。");
            }

            //保存到CurrentUSer
            var userTable = SystemConfig.UserTable.Document;
            var root = userTable.DocumentElement;
            var currentUser = userTable.CreateElement("CurrentUser");
            currentUser.SetAttribute("USID", usr.USID);
            root.AppendChild(currentUser);
            userTable.AppendChild(root);
            SystemConfig.UserTable.Save(userTable);
            return true;
        }

        public static void Logout() { }

        public static void Reg(String usid, String pwd, UserLevel userLevel = UserLevel.USER)
        {

            if (!File.Exists($"{Path.SystemConfigDir}\\{SystemConfig.USRCFG_NAME}"))
            {
                File.Create($"{Path.SystemConfigDir}\\{SystemConfig.USRCFG_NAME}").Close();
            }
            var xml = SystemConfig.UserTable;
            var root = xml.Document;
            var docroot = root.DocumentElement;
            var newusr = root.CreateElement("User");
            newusr.SetAttribute("USID", usid);
            newusr.SetAttribute("Password", pwd);
            newusr.SetAttribute("Level", userLevel.ToString());
            docroot.AppendChild(newusr);
            root.AppendChild(docroot);

            SystemConfig.UserTable.Save(root);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="UserAuthorizationException"></exception>
        public void CheckLevel()
        {
            if (CurrentUser.Level != UserLevel.OWNER)
            {
                throw new UserAuthorizationException("你没有权限完成此操作:\n\t新建用户\n该操作需要提升权限.");
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
