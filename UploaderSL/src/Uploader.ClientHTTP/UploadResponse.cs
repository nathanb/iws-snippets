using System;
using System.Runtime.Serialization;

namespace Uploader.ClientHttp
{
	[DataContract]
	public class UploadResponse : ResponseBase
	{
		[DataMember]
		public Guid Token { get; set; }
	}
}
