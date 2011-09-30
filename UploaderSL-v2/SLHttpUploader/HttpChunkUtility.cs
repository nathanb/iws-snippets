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
	public class HttpChunkUtility
	{
		System.Windows.Threading.Dispatcher dispatcher;
		IEnumerable<FileInfo> files;
		FilePostBehavior behavior;
		Dictionary<string, string> formData;
		string url;
		IEnumerator<FileInfo> enumerator;
		bool done;
		int uploadCount;
		int lastFileSequenceProgressReport;
		int lastFileContentProgressReport;
		int chunkSize = 32768;
		long bytesWritten = 0;

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

		public HttpChunkUtility()
		{
			done = false;
			lastFileSequenceProgressReport = 0;
			lastFileContentProgressReport = 0;
			uploadCount = 0;
		}

		public void PostFileContents(string url, IEnumerable<FileInfo> files, FilePostBehavior behavior, Dictionary<string, string> formData, System.Windows.Threading.Dispatcher dispatcher, int? chunkSize)
		{
			throw new NotImplementedException("chunk upload is broken");

			this.chunkSize = chunkSize ?? 32768;
			this.dispatcher = dispatcher;
			this.url = url;
			this.behavior = behavior;
			this.formData = formData;
			this.files = files;
			this.enumerator = files.GetEnumerator();

			bytesWritten = 0;
			StartUpload();
		}

		void StartUpload()
		{
			try
			{
				//next chunk
				OnFileSequenceProgressReport(0, 1);
				//write to POST request
				HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
				req.Method = "POST";
				var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
				req.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);
				var info = new PostRequestInfo() { FormData = formData, Request = req, Boundary = boundary, Dispatcher = dispatcher, Files=new List<FileInfo> { enumerator.Current } };
				req.BeginGetRequestStream(GetRequestStream, info);
			}
			catch (Exception ex)
			{
				OnUploadCompleted(false, ex);
			}
		}

		void FinishUpload(PostRequestInfo info)
		{
			//request thread
			this.dispatcher.BeginInvoke(() =>
			{
				uploadCount++;
				OnFileSequenceProgressReport(uploadCount, files.Count());
				StartUpload();
			});
		}
		//just write the contents of the stream to the output buffer. 
		void WriteFileContentRange(Stream destination, Stream source, string boundary, string filename, long fileSize, long contentStart)
		{
			var enc = new System.Text.UTF8Encoding(false, false);
			string contentTemplate = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"Chunk\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n",
				boundary, filename, DetermineMimeTypeFromExtension(filename));

			byte[] temp = enc.GetBytes(contentTemplate);
			destination.Write(temp, 0, temp.Length);

			byte[] buffer = new byte[this.chunkSize];
			int read = 1;
			source.Seek(contentStart, SeekOrigin.Begin);


			//write bytes from stream to output.
			read = source.Read(buffer, 0, buffer.Length);
			if (read > 0)
			{
				//test.Write(buffer, 0, read);
				destination.Write(buffer, 0, read);
				bytesWritten += read;
			}

			temp = enc.GetBytes("\r\n");
			destination.Write(temp, 0, temp.Length);
		}

		void UploadChunk(PostRequestInfo info)
		{
		}
		void FinishUploadChunk(PostRequestInfo info)
		{
		}

		private void GetRequestStream(IAsyncResult result)
		{
			var post = (PostRequestInfo)result.AsyncState;
			var enc = new System.Text.UTF8Encoding(false, false);

			try
			{
				using (var stream = post.Request.EndGetRequestStream(result))
				{
					var file = enumerator.Current;
					using (var reader = file.OpenRead())
					{
						WriteFileContentRange(stream, reader, post.Boundary, file.Name, file.Length, bytesWritten + 1 );
					}

					StringBuilder builder = new StringBuilder();
					//write form contents
					if (post.FormData != null)
					{
						string template = "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}";

						foreach (var field in post.FormData)
							builder.AppendFormat(template, post.Boundary, field.Key, field.Value);
					}

					builder.AppendFormat("\r\n--{0}--\r\n", post.Boundary);
					byte[] temp = enc.GetBytes(builder.ToString());
					stream.Write(temp, 0, temp.Length);
				}

				//get the response.
				post.Request.BeginGetResponse(GetResponse, post);

			}
			catch (Exception ex)
			{
				this.dispatcher.BeginInvoke(() => OnUploadCompleted(false, ex));
			}
		}
		private void GetResponse(IAsyncResult result)
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
				{
					throw new NotImplementedException("fix this");
					//if last chunk, then finish
					//else continue sending chunks.
					FinishUpload(post);
				}
				else
					this.dispatcher.BeginInvoke(() => OnUploadCompleted(false, new Exception("Upload failed. - " + json.Message)));
			}
			catch (Exception ex)
			{
				this.dispatcher.BeginInvoke(() => OnUploadCompleted(false, ex));
			}
		}

		protected void OnUploadCompleted(bool success, Exception error)
		{
			//all callers are in the UI thread. 
			if (uploadCompleted != null)
				uploadCompleted(new UploadCompletedEventArgs() { Error = error, Success = success });
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
