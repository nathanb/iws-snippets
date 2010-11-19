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

namespace Uploader.Client
{
	public class Utility
	{
		public const int chunkSize = 10000;

		public static string BaseUrl
		{
			get
			{
				string full = System.Windows.Application.Current.Host.Source.AbsoluteUri;
				string newpath = full.Substring(0, full.LastIndexOf('/')) + "/../";
				return newpath;
			}
		}
		public static string GetSHA256Hash(byte[] data)
		{
			if (data != null && data.Length > 0)
			{
				System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
				byte[] hash = sha.ComputeHash(data);

				return Convert.ToBase64String(hash);
			}
			return null;
		}
		public static string GetSHA256Hash(Stream data)
		{
			if (data != null && data.Length > 0)
			{
				System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
				byte[] hash = sha.ComputeHash(data);

				return Convert.ToBase64String(hash);
			}
			return null;
		}
	}
}
