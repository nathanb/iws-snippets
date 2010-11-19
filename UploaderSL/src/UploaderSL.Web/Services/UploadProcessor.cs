using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;

namespace UploaderSL.Web.Services
{
	[Serializable]
	public class UploadProcessor
	{
		public string Filename { get; set; }
		public Guid Token { get; set; }


		public UploadProcessor()
		{

			string folder = HostingEnvironment.MapPath(Config.StorageFolder);
			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);

			//get a unique name. 
			this.Token = Guid.NewGuid();
			this.Filename = Path.Combine(folder, this.Token.ToString());

			//create placeholder
			File.Create(this.Filename).Close();
		}

		public UploadProcessor(Guid token)
		{
			this.Token = token;
			string folder = HostingEnvironment.MapPath(Config.StorageFolder);

			//get a unique name. 
			this.Filename = Path.Combine(folder, this.Token.ToString());
		}

		public bool ProcessChunk(byte[] chunk, string hash)
		{
			//compare chunk hash. 
			if (hash == Utility.GetHashSHA256(chunk))
			{
				using (FileStream fs = File.Open(this.Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
				{
					long ix = fs.Seek(fs.Length, SeekOrigin.Begin);
					fs.Write(chunk, 0, chunk.Length);
					fs.Flush();
					fs.Close();
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// rehashes the whole file and compares it to the original.   this is purely optional. The file is already filled and closed at this point.
		/// </summary>
		/// <returns></returns>
		public bool Finish(string hash, string extension)
		{
			string newHash = null;
			using (FileStream fs = new FileStream(this.Filename, FileMode.Open, FileAccess.Read))
			{
				newHash = Utility.GetHashSHA256(fs);
			}

			if (hash == newHash)
			{
				//rename file add extension. 
				string newFilename = Utility.GetRandomString(8) + extension;
				File.Move(this.Filename, Path.Combine(Path.GetDirectoryName(this.Filename), newFilename));
				this.Filename = newFilename;
			}
			return true;
		}
	}
}
