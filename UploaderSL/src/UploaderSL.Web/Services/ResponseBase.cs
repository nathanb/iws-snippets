using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace UploaderSL.Web.Services
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
