using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;

namespace UploaderSL.Web
{
	public class Utility
	{
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

		public static string Serialize(object instance, Encoding encoding, SerializationOptions options)
		{
			bool omit_declaration = (options & SerializationOptions.OmitDeclaration) == SerializationOptions.OmitDeclaration;
			bool exclude_namespaces = (options & SerializationOptions.ExcludeNamespaces) == SerializationOptions.ExcludeNamespaces;

			string xml = null;

			XmlSerializer xs = new XmlSerializer(instance.GetType());
			using (MemoryStream ms = new MemoryStream())
			{
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Encoding = encoding;
				settings.OmitXmlDeclaration = omit_declaration;

				XmlWriter xw = XmlTextWriter.Create(ms, settings);
				if (exclude_namespaces)
				{
					XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
					namespaces.Add("", "");
					xs.Serialize(xw, instance, namespaces);
				}
				else
					xs.Serialize(xw, instance);

				ms.Flush();
				xml = encoding.GetString(ms.ToArray());
				ms.Close();
			}

			return xml;
		}

		/// <summary>
		/// Serialize an object to an xml string with specified encoding. 
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string Serialize(object instance, Encoding encoding)
		{
			return Serialize(instance, encoding, SerializationOptions.Default);
		}
		/// <summary>
		/// Serialize an object to XML with default encoding UTF8
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static string Serialize(object instance)
		{
			return Serialize(instance, Encoding.UTF8, SerializationOptions.Default);
		}

		/// <summary>
		/// Deserialize a stream source with specified encoding
		/// </summary>
		/// <param name="source"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static object Deserialize(Type type, Stream source, Encoding encoding)
		{
			object result = null;
			using (StreamReader sr = new StreamReader(source, encoding))
			{
				string xml = sr.ReadToEnd();
				XmlSerializer xs = new XmlSerializer(type);
				result = xs.Deserialize(sr);
			}
			return result;
		}

		/// <summary>
		/// Deserialize a stream source with default encoding UTF8
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static object Deserialize(Type type, Stream source)
		{
			return Deserialize(type, source, Encoding.UTF8);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static object Deserialize(Type type, string xml)
		{
			object result = null;

			XmlSerializer xs = new XmlSerializer(type);
			using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
			{
				result = xs.Deserialize(ms);
			}

			return result;
		}

	}
}
