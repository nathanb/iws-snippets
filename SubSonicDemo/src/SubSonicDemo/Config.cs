using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SubSonicDemo
{
	public class Config
	{
		public static string DefaultConnectionStringName { get { return ConfigurationManager.AppSettings["DefaultConnectionStringName"]; } }
		public static bool RunMigrations { get { return Convert.ToBoolean(ConfigurationManager.AppSettings["RunMigrations"]); } }
	}
}
