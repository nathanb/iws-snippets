using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace SLHttpUploader
{
	public class HttpChunkUtility : IHttpUtility
	{
		System.Windows.Threading.Dispatcher dispatcher;
		FilePostBehavior behavior;
		IDictionary<string, string> formData;
		string url;
		IEnumerator<FileInfo> enumerator;
		int lastFileSequenceProgressReport;
		int lastFileContentProgressReport;
		int chunkSize = 204800;
		int filesTransferred = 0;
		int filesToTransfer = 0;
		long currentFilePosition = 0;
		Guid currentFileId;
		bool sentEndChunk = false;

		#region events
		private event Action<UploadCompletedEventArgs> uploadCompleted;
		public event Action<UploadCompletedEventArgs> UploadCompleted
		{
			add { uploadCompleted += value; }
			remove { uploadCompleted -= value; }
		}

		private event Action<ProgressReportEventArgs> fileContentProgressReport;
		public event Action<ProgressReportEventArgs> FileContentProgressReport
		{
			add { fileContentProgressReport += value; }
			remove { fileContentProgressReport -= value; }
		}

		private event Action<ProgressReportEventArgs> fileSequenceProgressReport;
		public event Action<ProgressReportEventArgs> FileSequenceProgressReport
		{
			add { fileSequenceProgressReport += value; }
			remove { fileSequenceProgressReport -= value; }
		}
		#endregion

		public HttpChunkUtility(int? chunkSize)
		{
			this.chunkSize = chunkSize ?? 204800;
			lastFileSequenceProgressReport = 0;
			lastFileContentProgressReport = 0;
			currentFilePosition = 0;
		}

		public void PostFileContents(string url, IEnumerable<FileInfo> files, FilePostBehavior behavior, IDictionary<string, string> formData, System.Windows.Threading.Dispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
			this.url = url;
			this.behavior = behavior;
			this.formData = formData ?? new Dictionary<string, string>();
			this.enumerator = files.GetEnumerator();
			this.filesToTransfer = files.Count();
			this.filesTransferred = 0;

			if (enumerator.MoveNext())
				StartNextFile();
			else
				OnUploadCompleted(true, null);
		}

		private void StartNextFile()
		{
			OnFileSequenceProgressReport(filesTransferred, filesToTransfer);

			//clear from last round. 
			formData.Remove("Hash");//base64 SHA256 hash of original file. for finish request only. 
			formData.Remove("Filename"); //only for finish call

			currentFileId = Guid.NewGuid(); //set the new guid for this file.
			formData["Sequence"] = ChunkSequence.Body.ToString(); // finish == end. 
			currentFilePosition = 0L;
			sentEndChunk = false; //prep for new file

			StartNextChunk();
		}

		private void StartNextChunk()
		{
			OnFileContentProgressReport(currentFilePosition, enumerator.Current.Length);

			if (sentEndChunk)
			{
				filesTransferred++;
				if (enumerator.MoveNext())
					StartNextFile();
				else
					OnUploadCompleted(true, null);
			}
			else if (enumerator.Current.Length - chunkSize < currentFilePosition)
				StartEndChunk();
			else
				StartRequestForNextChunk();
		}

		private void StartEndChunk()
		{
			sentEndChunk = true;

			using (var fs = enumerator.Current.OpenRead())
				formData["Hash"] = Utility.GetSHA256Hash(fs);

			formData["Filename"] = enumerator.Current.Name;
			formData["Sequence"] = ChunkSequence.End.ToString();

			StartRequestForNextChunk();
		}



		//just write the contents of the stream to the output buffer. 

		private void StartRequestForNextChunk()
		{
			//next chunk
			//write to POST request
			HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
			req.Method = "POST";
			var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
			req.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
			var info = new PostRequestInfo() { FormData = formData, Request = req, Boundary = boundary, Dispatcher = dispatcher, Files = new List<FileInfo> { enumerator.Current } };
			req.BeginGetRequestStream(WriteRequestStream, info);
		}
		private void WriteRequestStream(IAsyncResult result)
		{
			try
			{
				var post = (PostRequestInfo)result.AsyncState;
				var enc = new System.Text.UTF8Encoding(false, false);

				using (var stream = post.Request.EndGetRequestStream(result))
				{
					//write file content
					var file = enumerator.Current;
					using (var reader = file.OpenRead())
						WriteFileContentRange(stream, reader, post.Boundary); //use the GuiD fileId as the filename. server-side will concatenate this file. 

					//write form data.
					StringBuilder builder = new StringBuilder();
					//write form contents
					if (post.FormData != null)
					{
						string template = "\r\n--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}";

						foreach (var field in post.FormData)
							builder.AppendFormat(template, post.Boundary, field.Key, field.Value);
					}

					builder.AppendFormat("\r\n--{0}--\r\n", post.Boundary);
					byte[] temp = enc.GetBytes(builder.ToString());
					stream.Write(temp, 0, temp.Length);
				}

				//wait for the response.
				post.Request.BeginGetResponse(ReceiveResponseCallback, post);
			}
			catch (Exception ex)
			{
				OnUploadCompleted(false, ex.Message);
			}
		}
		private void ReceiveResponseCallback(IAsyncResult result)
		{
			try
			{
				var post = (PostRequestInfo)result.AsyncState;
				HttpWebResponse response = (HttpWebResponse)post.Request.EndGetResponse(result);

				string responseText = null;
				var enc = new UTF8Encoding(false, false);
				using (var stream = response.GetResponseStream())
				{
					using (var reader = new StreamReader(stream, enc))
						responseText = reader.ReadToEnd();
				}

				JsonResponse json = null;
				if (!string.IsNullOrEmpty(responseText))
				{
					using (MemoryStream ms = new MemoryStream(enc.GetBytes(responseText)))
					{
						var js = new DataContractJsonSerializer(typeof(JsonResponse));
						json = js.ReadObject(ms) as JsonResponse;
					}
				}

				if (json != null && json.Success)
					StartNextChunk();
				else
					throw new Exception("Upload failed. - " + json.Message);
			}
			catch (Exception ex)
			{
				OnUploadCompleted(false, ex.Message);
			}
		}
		private void WriteFileContentRange(Stream destination, Stream source, string boundary)
		{
			var filename = currentFileId.ToString();
			
			var enc = new System.Text.UTF8Encoding(false, false);
			string contentTemplate = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"Chunk\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n",
				boundary, filename, DetermineMimeTypeFromExtension(enumerator.Current.Name)); //use original filename.

			byte[] temp = enc.GetBytes(contentTemplate);
			destination.Write(temp, 0, temp.Length);

			byte[] buffer = new byte[this.chunkSize];
			int read = 1;
			source.Seek(currentFilePosition, SeekOrigin.Begin); //start on next byte.  read one chunk.


			//write bytes from stream to output.
			read = source.Read(buffer, 0, buffer.Length);
			if (read > 0)
			{
				//test.Write(buffer, 0, read);
				destination.Write(buffer, 0, read);
				currentFilePosition += read;//TODO: may need to add +1 here. 
			}
		}

		#region Event Triggers
		protected void OnUploadCompleted(bool success, string message)
		{
			//all callers are in the UI thread. 
			if (uploadCompleted != null)
				this.dispatcher.BeginInvoke(() => uploadCompleted(new UploadCompletedEventArgs() { Message = message, Success = success }));
		}
		protected void OnFileSequenceProgressReport(long current, long total)
		{
			//consider skipping a number of calls for performance.  We don't ned to report b ack to the UI every chunk uploaded. 
			//The caller is in the request thread. 
			Action<long, long> action = (c, t) =>
			{
				var arg = new ProgressReportEventArgs() { Current = c, Total = t };
				var perc = arg.Percentage;
				if (perc == 0 || perc != lastFileSequenceProgressReport)
				{
					lastFileSequenceProgressReport = perc;
					if (fileSequenceProgressReport != null)
						fileSequenceProgressReport(arg);
				}
			};

			this.dispatcher.BeginInvoke(action, current, total);
		}
		protected void OnFileContentProgressReport(long current, long total)
		{
			//consider skipping a number of calls for performance.  We don't ned to report b ack to the UI every chunk uploaded. 
			//The caller is in the request thread. 
			Action<long, long> action = (c, t) =>
			{
				var arg = new ProgressReportEventArgs() { Current = c, Total = t };
				var perc = arg.Percentage;
				if (perc == 0 || perc != lastFileContentProgressReport)
				{
					lastFileContentProgressReport = perc;
					if (fileContentProgressReport != null)
						fileContentProgressReport(arg);
				}
			};

			this.dispatcher.BeginInvoke(action, current, total);
		}
		#endregion

		static string DetermineMimeTypeFromExtension(string filename)
		{
			var ext = System.IO.Path.GetExtension(filename).ToLower();
			var match = MIME_TYPES.Where(o => o.Value.Contains(ext)).Select(o => o.Key).FirstOrDefault();

			if (match != null)
				return match;
			return "application/octet-stream"; //default;
		}
		public static readonly Dictionary<string, List<string>> MIME_TYPES = new Dictionary<string, List<string>>()
		{
			{"text/plain", new List<string>() {".txt"}},
			{"text/html", new List<string>() {".html", ".htm"}},
			{"text/xml", new List<string>() {".xml"}},
			{"text/richtext", new List<string>() {".rtf"}},
			{"audio/x-aiff", new List<string>() {".aiff"}},
			{"audio/basic", new List<string>() {".basic"}},
			{"audio/mid", new List<string>() {".mid"}},
			{"audio/wav", new List<string>() {".wav"}},
			{"image/gif", new List<string>() {".gif"}},
			{"image/jpeg", new List<string>() {".jpg",".jpeg"}},
			{"image/png", new List<string>() {".png"}},
			{"image/x-png", new List<string>() {".png"}},
			{"image/tiff", new List<string>() {".tif", "tiff"}},
			{"image/bmp", new List<string>() {".bmp"}},
			{"image/x-emf", new List<string>() {".emf"}},
			{"image/x-wmf", new List<string>() {".wmf"}},
			{"video/avi", new List<string>() {".avi"}},
			{"video/mpeg", new List<string>() {".mpeg", ".mpg"}},
			{"application/postscript", new List<string>() {".ps"}},
			{"application/pdf", new List<string>() {".pdf"}},
			{"application/xml", new List<string>() {".xml"}},
			{"application/rss+xml", new List<string>() {".rss"}},
			{"application/x-zip-compressed", new List<string>() {".zip"}},
			{"application/x-gzip-compressed", new List<string>() {".gz"}}
		};
	}
}
