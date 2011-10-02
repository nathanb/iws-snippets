using System;
using System.IO;
using System.Collections.Generic;
namespace SLHttpUploader
{
	public interface IHttpUtility
	{
		event Action<ProgressReportEventArgs> FileContentProgressReport;
		event Action<ProgressReportEventArgs> FileSequenceProgressReport;
		void PostFileContents(string url, System.Collections.Generic.IEnumerable<FileInfo> files, FilePostBehavior behavior, IDictionary<string, string> formData, System.Windows.Threading.Dispatcher dispatcher);
		event Action<UploadCompletedEventArgs> UploadCompleted;
	}
}
