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
			var htmlWriter = new HtmlTextWriter(new StringWriter());

			return htmlWriter.InnerWriter.ToString();
		}
	}


}
