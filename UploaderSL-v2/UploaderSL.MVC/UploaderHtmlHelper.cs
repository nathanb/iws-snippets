using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;
using System.Web.UI;

namespace UploaderSL.MVC
{
	public static class UploaderHtmlHelper
	{
		public static string UploaderSL(this HtmlHelper helper)
		{
			return UploaderSL(helper, "recaptcha", "default", null);
		}

		public static string UploaderSL(this HtmlHelper helper, string id, string theme, string language)
		{
			//var captchaControl = new Recaptcha.RecaptchaControl();
			var htmlWriter = new HtmlTextWriter(new StringWriter());

			//aptchaControl.RenderControl(htmlWriter);

			return htmlWriter.InnerWriter.ToString();
		}
	}


}
