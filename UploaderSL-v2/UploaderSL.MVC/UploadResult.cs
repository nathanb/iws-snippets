using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UploaderSL.MVC
{
	public class ChunkUploadResult
	{
		public bool FileUploadComplete { get; set; }
		public bool Success { get; set; }
		public string Message { get; set; }
		public string TempFilePath { get; set; }
		public string OriginalFilename { get; set; }
	}
}
