using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JsonHostWebsite.Models
{
	public class CustomObject
	{
		public bool Success { get; set; }
		public object Data { get; set; }
		public string Message { get; set; }
	}
}