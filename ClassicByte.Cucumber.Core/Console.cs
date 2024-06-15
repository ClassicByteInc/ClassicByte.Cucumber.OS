using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassicByte.Cucumber.Core
{
    /// <summary>
    /// 对控制台的操作
    /// </summary>
    public static class Console
    {
        /// <summary>
        /// 向标准输出输出一个换行符
        /// </summary>
        public static void WriteLine()
        {
            System.Console.WriteLine();
        }

        /// <summary>
        /// 向标准输出输出指定内容
        /// </summary>
        public static void WriteLine(string text)
        {
            System.Console.WriteLine(text);
        }

        /// <summary>
        /// 向标准输出输出指定内容，并指定颜色
        /// </summary>
        public static void WriteLine(String text, System.ConsoleColor color)
        {
            var temp = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color;
            Console.WriteLine(text);
            System.Console.ForegroundColor = temp;
        }

        /// <summary>
        /// 向标准输入获取一行输入
        /// </summary>
        public static String ReadLine()
        {
            return System.Console.ReadLine();
        }
    }
}
