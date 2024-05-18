using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte.Cucumber.Core
{
    public static class Path
    {

        static readonly String SystemDirName = "CUCUMBERSYSTEMDIR";

        static Path()
        {
            if ((Environment.GetEnvironmentVariable(SystemDirName)==null)||(Environment.GetEnvironmentVariable(SystemDirName)==String.Empty))
            {
                Environment.SetEnvironmentVariable("CUCUMBERSYSTEMDIR", $"{new FileInfo(Process.GetCurrentProcess().MainModule.FileName).Directory.Parent.FullName}",EnvironmentVariableTarget.User);
            }

            Plugins.Create();
        }

        public static DirectoryInfo SystemDir => new DirectoryInfo($"{Environment.GetEnvironmentVariable("CUCUMBERSYSTEMDIR")}");

        public static DirectoryInfo SystemConfig => new DirectoryInfo($"{SystemDir.FullName}\\Config");

        public static DirectoryInfo SystemCoreDir => new DirectoryInfo($"{SystemDir.FullName}\\Core");

        public static DirectoryInfo Plugins => new DirectoryInfo($"{SystemDir.FullName}\\plugins");

        public static DirectoryInfo Temp { get {
                return Directory.CreateDirectory($"{SystemDir.FullName}\\Temp");
            } }
    }
}
