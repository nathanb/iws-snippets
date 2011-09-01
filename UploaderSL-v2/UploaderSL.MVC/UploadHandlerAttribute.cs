using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace UploaderSL.MVC
{
	public class UploadHandlerAttribute : ActionFilterAttribute
	{
		private const string UPLOAD_FIELD_KEY = "upload_field";

		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			//handle the upload. 

			// this will push the result values into a parameter in our Action
			filterContext.ActionParameters["files"] = null; //custom route params

			base.OnActionExecuting(filterContext);
		}
	}
}
