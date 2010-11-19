using System;
using System.Runtime.Serialization;

namespace Uploader.ClientHttp
{
	[DataContract]
	public class FinishRequest
	{
		[DataMember]
		public string FullHash { get; set; }
		[DataMember]
		public string Extension { get; set; }
		[DataMember]
		public Guid Token { get; set; }
	}
}
