using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UploaderSL.Web
{
	public class Config
	{
		public static string StorageFolder
		{
			get
			{
				return System.Configuration.ConfigurationManager.AppSettings["StorageFolder"];
			}
		}
	}
}
