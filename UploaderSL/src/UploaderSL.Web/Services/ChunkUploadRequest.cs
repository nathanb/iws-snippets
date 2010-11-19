using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace UploaderSL.Web.Services
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
