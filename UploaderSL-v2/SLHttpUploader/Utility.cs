using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SLHttpUploader
{
	public class Utility
	{
		public static string BaseUrl
		{
			get
			{
				string full = System.Windows.Application.Current.Host.Source.AbsoluteUri;
				string newpath = full.Substring(0, full.LastIndexOf('/')) + "/../";
				return newpath;
			}
		}
	}
}
