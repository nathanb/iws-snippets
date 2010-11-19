using System;
using System.Runtime.Serialization;

namespace Uploader.ClientHttp
{
	[DataContract]
	public class FinishResponse : ResponseBase
	{
		[DataMember]
		public string NewFilename { get; set; }
	}
}
