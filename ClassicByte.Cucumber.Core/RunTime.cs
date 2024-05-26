using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte.Cucumber.Core
{
    internal class RunTime
    {
        public const String USRCFGKEY = "CLASSICBYTE_CUC_USR";

        static RunTime()
        {
            Directory.CreateDirectory(Path.Plugins.FullName);
            Directory.CreateDirectory(Path.SystemDir.FullName);
            Directory.CreateDirectory(Path.Plugins.FullName);
            Directory.CreateDirectory(Path.SystemCoreDir.FullName);
            Directory.CreateDirectory(Path.SystemConfigDir.FullName);
        }
    }
}
