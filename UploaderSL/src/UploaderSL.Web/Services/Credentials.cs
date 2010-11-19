using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace UploaderSL.Web.Services
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
