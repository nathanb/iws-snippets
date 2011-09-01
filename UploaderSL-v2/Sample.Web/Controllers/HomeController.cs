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
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public JsonResult Upload() //optionally, custom data can be plugged in here 
		{
			bool success = false;

			//handle these uploads the same as you would a normal File post. 
			try
			{
				for (int ix = 0; ix < Request.Files.Count; ix++)
				{
					var file = Request.Files[ix];

					//TODO: file type check; don't accept executables or scripts
					var filename = Path.GetFileName(file.FileName);

					//TODO: pick a destination folder. 
					//TODO: make sure the destination is a read-only & (non-executable) path to anonymous visitors & app pool identity
					var destination = Server.MapPath("~/App_Data/uploads/"); 


					if (!Directory.Exists(destination))
						Directory.CreateDirectory(destination);

					var destinationFile = Path.Combine(destination, filename);

					//write the data.
					using (var fs = System.IO.File.Open(destinationFile, FileMode.Create, FileAccess.Write))
					{
						int read = 4096;
						byte[] buffer = new byte[read];
						while (read > 0)
						{
							read = file.InputStream.Read(buffer, 0, buffer.Length);
							if (read > 0)
								fs.Write(buffer, 0, read);
						}
					}
				}

				success = true;
			}
			catch (Exception ex)
			{
				//TODO: logging or failure response.
			}

			return Json(new { Success=success });
		}
	}
}
