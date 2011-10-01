using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;

namespace SLHttpUploader
{
	public class Utility
	{
		public static string BaseUrl
		{
			get
			{
				string full = System.Windows.Application.Current.Host.Source.AbsoluteUri;
				string newpath = full.Substring(0, full.LastIndexOf('/')) + "/../";
				return newpath;
			}
		}

		public static string GetSHA256Hash(Stream source)
		{
			var sha = new System.Security.Cryptography.SHA1Managed();
			byte[] hash = sha.ComputeHash(source);
			return Convert.ToBase64String(hash);
		}
	}
}
