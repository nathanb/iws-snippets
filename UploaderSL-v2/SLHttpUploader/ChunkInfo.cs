using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLHttpUploader
{
	public class ChunkInfo
	{
		public ChunkSequence Sequence { get; set; }
		public byte[] Chunk { get; set; }
		public string Hash { get; set; }
	}
}