using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace SLHttpUploader
{
	public class PostRequestInfo
	{
		public IEnumerable<FileInfo> Files { get; set; }
		public Dictionary<string, string> FormData { get; set; }
		public FilePostBehavior Behavior { get; set; }
		public WebRequest Request { get; set; }
		public string Boundary { get; set; }
		public System.Windows.Threading.Dispatcher Dispatcher { get; set; }

		//by chunk
		public long ChunkProgress { get; set; }
		public byte[] FileChunk { get; set; }
	}
}
