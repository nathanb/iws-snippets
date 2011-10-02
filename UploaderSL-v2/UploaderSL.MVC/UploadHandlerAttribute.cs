using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;

namespace UploaderSL.MVC
{
	public class ChunkUploadHandlerAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var uploadResult = new ChunkUploadResult(); 

			string path = null;
			try
			{
				if (filterContext.HttpContext.Request.Files.Count > 0)
				{
					var sseq = filterContext.HttpContext.Request["Sequence"];
					var seq = ChunkSequence.Body;
					if (!string.IsNullOrEmpty(sseq))
						System.Enum.TryParse<ChunkSequence>(sseq, out seq);

					var post = new ChunkInfo() { Sequence = seq, Hash = filterContext.HttpContext.Request["Hash"], Filename = filterContext.HttpContext.Request["Filename"]  };
					var postedFile = filterContext.HttpContext.Request.Files[0];
					path = filterContext.HttpContext.Server.MapPath("~/App_Data/" + postedFile.FileName); //using filename as the identifier (GUID)
					AppendPostedFileContent(postedFile, path);
					
					if (post.Sequence == ChunkSequence.End)
					{
						//validate full file hash. 
						string hash = null;
						using (var fs = System.IO.File.OpenRead(path))
							hash = Utility.GetHashSHA1(fs);

						if (hash == post.Hash)
						{
							uploadResult.Success = true;
							uploadResult.FileUploadComplete = true;
							uploadResult.OriginalFilename = post.Filename;
							uploadResult.TempFilePath = path;
						}
						else
						{
							//fail...resend.
							System.IO.File.Delete(path); //cleanup
							uploadResult.Success = false;
							uploadResult.Message = "Hash fail";

						}
					}
					else
						uploadResult.Success = true;
				}
			}
			catch (Exception ex)
			{
				try
				{
					if (System.IO.File.Exists(path))
						System.IO.File.Delete(path);
				}
				catch { } //swallow it. if it's write permissions, we already know about it. this is just an attempt to cleanup.

				uploadResult.Success = false;
				uploadResult.Message = ex.Message;
			}

			filterContext.ActionParameters["result"] = uploadResult;

			base.OnActionExecuting(filterContext);
		}

		private void AppendPostedFileContent(HttpPostedFileBase postedData, string path)
		{
			byte[] buffer = new byte[32 * 1024];
			int read = 1;

			//check to ensure directory exists first. 
			string dir = Path.GetDirectoryName(path);
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			using (var fs = System.IO.File.Open(path, FileMode.Append, FileAccess.Write, FileShare.None))
			{
				while (read > 0)
				{
					read = postedData.InputStream.Read(buffer, 0, buffer.Length);
					if (read > 0)
						fs.Write(buffer, 0, read);
				}
			}
		}
	}
}
