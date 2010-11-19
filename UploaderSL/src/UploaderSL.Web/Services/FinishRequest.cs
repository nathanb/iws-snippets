using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace UploaderSL.Web.Services
{
	[DataContract]
	public class FinishRequest
	{
		[DataMember]
		public Credentials Credentials { get; set; }
		[DataMember]
		public string FullHash { get; set; }
		[DataMember]
		public string Extension { get; set; }
		[DataMember]
		public Guid Token { get; set; }
	}
}
