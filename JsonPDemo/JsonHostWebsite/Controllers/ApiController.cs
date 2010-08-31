using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace JsonHostWebsite.Controllers
{
	public class ApiController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}
		/// <summary>
		/// Get some basic information with a JSONP GET request. 
		/// </summary>
		/// <remarks>
		///	Sample url: 
		///	http://localhost:50211/Api/GetInformation?key=test&callback=json123123
		/// </remarks>
		/// <param name="key"></param>
		/// <returns></returns>
		public JsonpResult GetInformation(string key)
		{
			var resp = new Models.CustomObject();
			if (validateKey(key))
			{
				resp.Data = "you provided key: " + key;
				resp.Success = true;
			}
			else
				resp.Message = "unauthorized";

			return this.Jsonp(resp);
			//using extension method
		}

		bool validateKey(string key)
		{
			return true; 
		}
	}
}
