﻿using System;
using ClassicByte.Cucumber.Core.Exceptions;

namespace ClassicByte.Cucumber.App.NetShell
{
    internal class Program
    {
        public static void Main()
        {
			try
			{

			}
			catch (Error e)
			{
				e.Print();
				throw;
			}
        }
    }
}
