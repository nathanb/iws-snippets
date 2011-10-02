using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.IO;
using System.Windows.Browser;

namespace SLHttpUploader
{
	public partial class MainPage : UserControl
	{
		Settings currentSettings;
		int sequenceProgressCalls = 0;
		int contentProgressCalls = 0;

		public MainPage()
		{
			InitializeComponent();
			currentSettings = new Settings() { MaxUploadSize = 4096, UploadFilesIndividually = true }; //default for asp.net

			HtmlPage.RegisterScriptableObject("page", this);
		}

		private void uxSelectFiles_Click(object sender, RoutedEventArgs e)
		{
			var of = new OpenFileDialog();
			of.Multiselect = true;
			of.Filter = "All Files (*.*)|*.*";
			if (of.ShowDialog() ?? false)
			{
				long total = 0;
				if (currentSettings.UploadFilesIndividually)
					total = of.Files.Max(o => o.Length);
				else
					total = of.Files.Sum(o => o.Length);

				//this amount doesn't consider message overhead size; so really we could probably add 200-300 bytes to the file sizes and compare against that. 
				total += 300; //this is just an arbitrary amount; semi-safe guess.
				total /= 1024; //convert to KB

				if (total <= currentSettings.MaxUploadSize)
				{
					var utility = new HttpChunkUtility();
					utility.UploadCompleted += new Action<UploadCompletedEventArgs>(utility_UploadCompleted);
					utility.FileSequenceProgressReport += new Action<ProgressReportEventArgs>(utility_FileSequenceProgressReport);
					utility.FileContentProgressReport += new Action<ProgressReportEventArgs>(utility_FileContentProgressReport);

					callScriptStartup();
					utility.PostFileContents(currentSettings.PostUrl,
						of.Files, currentSettings.UploadFilesIndividually ? FilePostBehavior.OneAtATime : FilePostBehavior.AllAtOnce,
						currentSettings.CustomData,
						this.Dispatcher, null); //default chunk
				}
				else
					utility_UploadCompleted(new UploadCompletedEventArgs() { Success = false, Error = new Exception("File size too large.") });
			}
		}

		void utility_FileContentProgressReport(ProgressReportEventArgs e)
		{
			contentProgressCalls++;
			HtmlPage.Window.Invoke(currentSettings.ScriptContentProgressHandler, e.Percentage);
		}
		void callScriptStartup()
		{
			HtmlPage.Window.Invoke(currentSettings.ScriptStartupHandler);
		}
		void utility_FileSequenceProgressReport(ProgressReportEventArgs e)
		{
			sequenceProgressCalls++;
			HtmlPage.Window.Invoke(currentSettings.ScriptSequenceProgressHandler, e.Percentage);
		}

		void utility_UploadCompleted(UploadCompletedEventArgs e)
		{
			HtmlPage.Window.Invoke(currentSettings.ScriptCompletedHandler, e.Success, e.Error != null ? e.Error.Message : string.Empty);
		}

		[ScriptableMember]
		public void Setup(string url, int maxUploadSize, bool uploadFilesIndividually, string sequenceProgressHandler, string contentProgressHandler, string completeHandler, string startupHandler, string customData, string buttonText)
		{
			if (!string.IsNullOrEmpty(buttonText))
				this.uxSelectFiles.Content = buttonText;
			currentSettings.MaxUploadSize = maxUploadSize;
			currentSettings.UploadFilesIndividually = uploadFilesIndividually;
			if (url.ToLower().Trim().StartsWith("http"))
				currentSettings.PostUrl = url;
			else
				currentSettings.PostUrl = Utility.BaseUrl + url.TrimStart('/');

			currentSettings.ScriptCompletedHandler = completeHandler;
			currentSettings.ScriptSequenceProgressHandler = sequenceProgressHandler;
			currentSettings.ScriptContentProgressHandler = contentProgressHandler;
			currentSettings.ScriptStartupHandler = startupHandler;

			//parse customData
			currentSettings.CustomData = parseCustomData(customData);
		}

		private Dictionary<string, string> parseCustomData(string data)
		{
			if (!string.IsNullOrEmpty(data))
			{
				var result = new Dictionary<string, string>();
				var properties = data.Split(';');
				foreach (var item in properties)
				{
					var parts = item.Split(':');
					result.Add(parts[0], parts[1]);
				}
				return result;
			}
			return null;
		}
	}
}
