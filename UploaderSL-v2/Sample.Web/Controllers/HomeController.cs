using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Sample.Web.Controllers
{
	public class HomeController : Controller
	{
		//
		// GET: /Home/

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult Upload()
		{
			try
			{
				if (Request.Files != null && Request.Files.Count > 0)
				{
					for (int ix = 0; ix < Request.Files.Count; ix++)
					{
						var file = Request.Files[ix];
						var path = Server.MapPath(string.Format("~/App_Data/{0}", file.FileName));
						var dir = Path.GetDirectoryName(path);
						if (!Directory.Exists(dir))
							Directory.CreateDirectory(dir);

						using (var fs = System.IO.File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
						{
							int read = 1;
							byte[] buffer = new byte[4096];
							while (read > 0)
							{
								read = file.InputStream.Read(buffer, 0, buffer.Length);
								if (read > 0)
									fs.Write(buffer, 0, read);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				return Json(new { Success=false, Message = ex.Message });
			} 
			return Json(new { Success = true });
		}

		[HttpPost]
		public JsonResult UploadChunked(ChunkInfo post)
		{
			try
			{
				if (Request.Files.Count > 0)
				{
					var file = Server.MapPath("~/App_Data/" + post.FileId.ToString());
					AppendFileContent(file);
					if (post.Sequence == ChunkSequence.End)
					{
						//validate full file hash. 
						string hash = null;
						using (var fs = System.IO.File.OpenRead(file))
							hash = TextUtility.GetHashSHA256(fs);
						if (hash == post.Hash)
						{
							//success... send file to storage.
							System.IO.File.Move(file, Server.MapPath("~/App_Data/" + post.Filename));
							return Json(new { Success = true });
						}
						else
						{
							//fail...resend.
							return Json(new { Success = false, Message = "Hash fail" });
						}
					}
				}
			}
			catch (Exception ex)
			{
				return Json(new { Success = false, Message = ex.Message });
			}
			return Json(new { Success = true });
		}

		private void AppendFileContent(string file)
		{
			var postedData = Request.Files[0];
			byte[] buffer = new byte[32*1024];
			int read = 1;

			using (var fs = System.IO.File.Open(file, FileMode.Append, FileAccess.Write, FileShare.None))
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
