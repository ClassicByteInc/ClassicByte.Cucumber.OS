using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ClassicByte.Cucumber.Core.Exceptions;

///<remarks></remarks>
namespace ClassicByte.Cucumber.Core.UserControl
{
    /// <summary>
    /// Cucumber 用户对象，提供操作用户的方法
    /// </summary>
    public class User
    {

        #region 私有变量
        #endregion

        public String Name { get; private set; }

        public String USID { get; private set; }

        public UserLevel Level { get; private set; }

        internal User(String usid,string name,UserLevel userLevel)
        {
            USID = usid;
            Name = name;
            Level = userLevel;
        }

        public static User CurrentUser
        {
            get
            {
                try
                {
                    var xml = SystemConfig.UserTable;
                    var currentUsr = xml.Document.GetElementById("CurrentUser");
                    var usid = currentUsr.GetAttribute("USID");
                    return FindUser(usid);
                }
                catch (NullReferenceException)
                {
                    return null;
                }
                catch (Exception e)
                {
                    throw new Error(e.Message,e.Message);
                }
            }
        }

        public static User FindUser(string usid)
        {
            var usrList = new List<String>();
            var usrs = SystemConfig.UserTable.Document.GetElementsByTagName("User");
            for (int i = 0; i < usrs.Count; i++)
            {
                usrList[i] = usrs[i].Attributes["USID"].Value; 
            }
            var target = usrs[usrList.IndexOf(usid)];
            var name = target.Attributes["Name"].Value;
            var usrid = target.Attributes["USID"].Value;
            UserLevel userLevel;
            switch (target.Attributes["LEVEL"].Value)
            {
                case "USER":
                    userLevel = UserLevel.USER; break;
                case "OWNER":
                    userLevel = UserLevel.OWNER; break;
                default:
                    throw new Exception($"未知的级别：{target.Attributes["LEVEL"].Value}");
            }
            return new User(usid,name,userLevel);
        }

        public void Login(String usid, String pwd) { }

        public void Logout() { }

        public void Reg(String usid ,String pwd,UserLevel userLevel = UserLevel.USER) {

            var xml = SystemConfig.UserTable;

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
