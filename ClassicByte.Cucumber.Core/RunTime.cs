using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte.Cucumber.Core
{
    public class RunTime 
    {
        internal const String USRCFGKEY = "CLASSICBYTE_CUC_USR";

        static RunTime()
        {
            Directory.CreateDirectory(Path.Plugins.FullName);
            Directory.CreateDirectory(Path.SystemDir.FullName);
            Directory.CreateDirectory(Path.Plugins.FullName);
            Directory.CreateDirectory(Path.SystemCoreDir.FullName);
            Directory.CreateDirectory(Path.SystemConfigDir.FullName);
            Directory.CreateDirectory(Path.Temp.FullName);
        }

        /// <summary>
        /// 系统退出时实现的方法委托
        /// </summary>
        public delegate void ExitMethod();

        /// <summary>
        /// 当系统退出时调用的方法
        /// </summary>
        public static void Dispose()
        {

        }

        /// <summary>
        /// 当系统退出时调用的方法，并指定自定义的方法
        /// </summary>
        /// <param name="exit"></param>
        public static void Dispose(ExitMethod exit )
        {
            exit();

            RunTime.Dispose();
        }
    }
}
