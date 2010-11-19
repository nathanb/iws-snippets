using System;
using System.Runtime.Serialization;

namespace Uploader.ClientHttp
{
	[DataContract]
	public class ResponseBase
	{
		[DataMember]
		public Enums.ResponsStatus Status { get; set; }

		[DataMember]
		public string Message { get; set; }
	}
}
