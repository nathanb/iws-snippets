using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SLHttpUploader
{
	[DataContract]
	public class JsonResponse
	{
		[DataMember]
		public bool Success { get; set; }
		[DataMember]
		public string Message { get; set; }
		[DataMember]
		public object Data { get; set; }
	}
}
