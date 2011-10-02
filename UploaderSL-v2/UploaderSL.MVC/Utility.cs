using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UploaderSL.MVC
{
	public static class Utility
	{
		public static string GetHashSHA256(Stream data)
		{
			System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
			byte[] result = sha.ComputeHash(data);
			return Convert.ToBase64String(result);
		}
		public static string GetHashSHA1(Stream data)
		{
			System.Security.Cryptography.SHA1Managed sha = new System.Security.Cryptography.SHA1Managed();
			byte[] result = sha.ComputeHash(data);
			return Convert.ToBase64String(result);
		}
	}
}
