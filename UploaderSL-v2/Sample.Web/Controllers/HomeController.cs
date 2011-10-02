using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using UploaderSL.MVC;

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
				return Json(new { Success = false, Message = ex.Message });
			}
			return Json(new { Success = true });
		}

		[HttpPost]
		[ChunkUploadHandler]
		public JsonResult UploadChunked(ChunkUploadResult result)
		{
			//this is important because the uploader will make multiple requests for this action. 
			//when the upload (for an individual file) is complete, check these two variables for a finished upload.
			if (result.Success && result.FileUploadComplete)
			{
				try
				{
					string path = Server.MapPath("~/App_Data/" + result.OriginalFilename);
					if (System.IO.File.Exists(path))
						System.IO.File.Delete(path);
					System.IO.File.Move(result.TempFilePath, path); //using the posted Filename in the last chunk request to determine final name. 
				}
				catch
				{
					//cleanup, remove temp file.
					try
					{
						if (System.IO.File.Exists(result.TempFilePath))
							System.IO.File.Delete(result.TempFilePath);
					}
					catch { } //swallow it. at this point, we're only attempting to cleanup an orphaned file. 
				}
			}
			return Json(new { Success = result.Success, Message = result.Message });
		}
	}
}
