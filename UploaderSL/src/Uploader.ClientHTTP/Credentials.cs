using System;
using System.Runtime.Serialization;

namespace Uploader.ClientHttp
{
	[DataContract]
	public class Credentials
	{
		[DataMember]
		public string Username { get; set; }
		[DataMember]
		public string Password { get; set; }
	}
}
