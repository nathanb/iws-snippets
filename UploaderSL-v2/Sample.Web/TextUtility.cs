using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Security.Cryptography;

namespace Sample.Web
{
	public static class TextUtility
	{
		public static string DecodeUTF8Binary(byte[] data)
		{
			if (data != null)
			{
				var enc = new UTF8Encoding(false, false);
				return enc.GetString(data);
			}
			return null;
		}

		public static byte[] EncodeUTF8Binary(string text)
		{
			if (string.IsNullOrEmpty(text))
				return null;

			var enc = new UTF8Encoding(false, false);
			return enc.GetBytes(text);
		}

		public static void SerializeXml(Stream output, object data)
		{
			XmlSerializer xs = new XmlSerializer(data.GetType());
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = new UTF8Encoding(false, false);
			XmlWriter writer = XmlTextWriter.Create(output, settings);
			xs.Serialize(writer, data);
			writer.Flush();
			writer.Close();
		}
		public static T DeserializeXml<T>(string xml) where T : class, new()
		{
			object result = null;

			XmlSerializer xs = new XmlSerializer(typeof(T));
			using (MemoryStream ms = new MemoryStream(new UTF8Encoding(false, false).GetBytes(xml)))
			{
				result = xs.Deserialize(ms);
			}

			return result as T;
		}

		public static string GetHashSHA256(byte[] data)
		{
			System.Security.Cryptography.SHA256Managed sha = new System.Security.Cryptography.SHA256Managed();
			byte[] result = sha.ComputeHash(data);
			return Convert.ToBase64String(result);
		}
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
		public static string GetRandomString(int length)
		{
			//returns random string of length specified with characters including: 0-9, a-z, A-Z
			char[] ca = new char[length];
			byte[] random = new Byte[length];
			//RNGCryptoServiceProvider is an implementation of a random number generator.

			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetBytes(random); // The array is now filled with cryptographically strong random bytes.

			for (int i = 0; i < length; i++)
			{
				bool found = false;
				int rand = (int)random[i];
				while (!found)
				{
					if (((rand >= 48) && (rand <= 57)) || ((rand >= 65) && (rand <= 90)) || ((rand >= 97) && (rand <= 122)))
					{
						found = true;
					}
					else
					{
						//get a new random int.
						byte[] single = new byte[1];
						rng.GetBytes(single);
						rand = single[0];
					}
				}
				char ci = (char)rand;
				ca[i] = ci;
			}
			string s = new string(ca);
			return s;
		}
		/// <summary>
		/// Gets the top element of an XML string.
		/// </summary>
		/// <param name="Xml">
		/// The XML string to extract the top element from.
		/// </param>
		/// <returns>
		/// The name of the first regular XML element.
		/// </returns>
		public static string GetTopElement(byte[] Xml)
		{
			using (MemoryStream ms = new MemoryStream(Xml))
			{
				return GetTopElement(ms);
			}
		}

		/// <summary>
		/// Gets the top element of an XML string.
		/// </summary>
		/// <param name="Xml">
		/// The XML string to extract the top element from.
		/// </param>
		/// <returns>
		/// The name of the first regular XML element.
		/// </returns>
		public static string GetTopElement(string Xml)
		{
			return GetTopElement(System.Text.Encoding.UTF8.GetBytes(Xml));
		}

		/// <summary>
		/// Gets the top element of an XML string.
		/// </summary>
		/// <param name="Xml">
		/// The XML string to extract the top element from.
		/// </param>
		/// <returns>
		/// The name of the first regular XML element.
		/// </returns>
		public static string GetTopElement(Stream Xml)
		{
			//we know that network streams can have issues so we may want to add
			//code to see if someone passed in a network stream
			//For now we are going to run unit tests and see if any issues come back

			//set the begin postion so we can set the stream back when we are done.
			long beginPos = Xml.Position;
			Xml.Position = 0;
			XmlTextReader XReader = new XmlTextReader(Xml);
			XReader.WhitespaceHandling = WhitespaceHandling.None;
			XReader.Read();
			XReader.Read();
			string RetVal = XReader.Name;
			//Do not close the stream, we will still need it for additional
			//operations.
			//XReader.Close();
			//reposition the stream to where it started
			Xml.Position = beginPos;
			return RetVal;
		}
		public enum SerializationOptions
		{
			Default = 0,
			ExcludeNamespaces = 1,
			OmitDeclaration = 2
		}

		public static string GetFriendlyTimeSpanName(TimeSpan ts)
		{
			if (ts.TotalDays >= 3650)
				return "never";
			else if (ts.TotalDays >= 730)
				return ((int)(ts.TotalDays / 365)).ToString() + " years ago";
			else if (ts.TotalDays > 365)
				return "a year ago";
			else if ((int)ts.TotalDays >= 60)
				return ((int)(ts.TotalDays / 30)).ToString() + " months ago";
			else if ((int)ts.TotalDays > 30)
				return "a month ago";
			else if ((int)ts.TotalDays > 1)
				return ((int)ts.TotalDays).ToString() + " days ago";
			else if ((int)ts.TotalDays > 0)
				return "yesterday";
			else if ((int)ts.TotalHours > 1)
				return ((int)ts.TotalHours).ToString() + " hours ago";
			else if ((int)ts.TotalHours > 0)
				return "an hour ago";
			else if ((int)ts.TotalMinutes > 1)
				return ((int)ts.TotalMinutes).ToString() + " minutes ago";
			else if ((int)ts.TotalMinutes > 0)
				return "a minute ago";

			return "just now";
		}

		public static string GetFriendlyByteSize(int size)
		{
			if (size >= 1024)
				return (size / 1024.0d).ToString("N0") + " kB";
			else if (size >= 1048576.0d)
				return (size / 1048576.0d).ToString("N1") + " MB";

			return size.ToString("N0") + " bytes";
		}
	}
}
