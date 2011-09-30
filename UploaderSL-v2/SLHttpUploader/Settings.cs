using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace SLHttpUploader
{
	public class Settings
	{
		public string PostUrl { get; set; }
		public bool UploadFilesIndividually { get; set; }
		public int MaxUploadSize { get; set; }
		public string ScriptSequenceProgressHandler { get; set; }
		public string ScriptContentProgressHandler { get; set; }
		public string ScriptCompletedHandler { get; set; }
		public string ScriptStartupHandler { get; set; }
		public Dictionary<string,string> CustomData { get; set; }
	}
}
