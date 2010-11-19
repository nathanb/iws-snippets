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
using System.Xml;
using System.Xml.Serialization;

namespace Uploader.ClientHttp
{
	public class Utility
	{
		public const int chunkSize = 102400;

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

		/// <summary>
		/// Serialize an object to UTF8 Xml
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string SerializeXml(object data)
		{
			string xml = null;
			using (MemoryStream ms = new MemoryStream())
			{
				XmlSerializer xs = new XmlSerializer(data.GetType());
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Encoding = System.Text.Encoding.UTF8;
				XmlWriter xw = XmlWriter.Create(ms, settings);
				xs.Serialize(xw, data);
				xml = System.Text.Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
			}
			return xml;
		}

		/// <summary>
		/// Deserialize an object from UTF8 Xml
		/// </summary>
		/// <param name="type"></param>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static object DeserializeXml(Type type, string xml)
		{
			object data = null;
			using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml)))
			{
				XmlSerializer xs = new XmlSerializer(type);
				XmlReader xr = XmlReader.Create(ms);
				if (xs.CanDeserialize(xr))
					data = xs.Deserialize(xr);
			}
			return data;
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
			XmlReader XReader = XmlReader.Create(Xml);
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
	}
}
