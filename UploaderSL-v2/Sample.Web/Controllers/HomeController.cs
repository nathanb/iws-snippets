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

		[HttpPost]
		public JsonResult Upload()
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
			string path = null;
			try
			{
				if (Request.Files.Count > 0)
				{
					var postedFile = Request.Files[0];
					path = Server.MapPath("~/App_Data/" + postedFile.FileName); //using filename as the identifier (GUID)
					AppendPostedFileContent(postedFile, path);
					if (post.Sequence == ChunkSequence.End)
					{
						//validate full file hash. 
						string hash = null;
						using (var fs = System.IO.File.OpenRead(path))
							hash = TextUtility.GetHashSHA1(fs);
						if (hash == post.Hash)
						{
							//success... send file to storage.
							var newPath = Server.MapPath("~/App_Data/" + post.Filename);
							if (System.IO.File.Exists(newPath))
								System.IO.File.Delete(newPath);
							System.IO.File.Move(path, newPath); //using the posted Filename in the last chunk request to determine final name. 
							return Json(new { Success = true });
						}
						else
						{
							//fail...resend.
							System.IO.File.Delete(path); //cleanup
							return Json(new { Success = false, Message = "Hash fail" });
						}
					}
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

				return Json(new { Success = false, Message = ex.Message });
			}
			return Json(new { Success = true });
		}

		private void AppendPostedFileContent(HttpPostedFileBase postedData, string path)
		{
			byte[] buffer = new byte[32*1024];
			int read = 1;

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
