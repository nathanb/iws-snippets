using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace UploaderSL.Web.Services
{
	[DataContract]
	public class FinishResponse : ResponseBase
	{
		[DataMember]
		public string NewFilename { get; set; }
	}
}
