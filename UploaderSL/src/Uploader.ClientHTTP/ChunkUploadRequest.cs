using System;
using System.Runtime.Serialization;

namespace Uploader.ClientHttp
{
	[DataContract]
	public class ChunkUploadRequest
	{
		[DataMember]
		public Guid Token { get; set; }
		[DataMember]
		public string Hash { get; set; }
		[DataMember]
		public byte[] Chunk { get; set; }
	}
}
