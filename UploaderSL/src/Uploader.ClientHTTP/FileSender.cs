using System;
using System.Net;
using System.IO;
using System.Text;

namespace Uploader.ClientHttp
{
	public sealed class FileSender : IDisposable
	{
		public enum SendStatus
		{
			Cancelled = 1,
			Failed = 2,
			Success = 3
		}

		FileInfo _File;

		private event FileSenderProgressChangedEventHandler _ProgressChanged;
		public event FileSenderProgressChangedEventHandler ProgressChanged
		{
			add { _ProgressChanged += value; }
			remove { _ProgressChanged -= value; }
		}
		private event FileSenderCompletedEventHandler _Completed;
		public event FileSenderCompletedEventHandler Completed
		{
			add { _Completed += value; }
			remove { _Completed -= value; }
		}


		public int Steps { get; set; }
		public int CurrentStep { get; set; }
		public long FileLength { get; set; }
		public long FilePosition { get; set; }
		public Guid Token { get; set; }
		public int ChunkRetryAttempts { get; set; }
		public Uri Uri { get; set; }

		private FileStream _FileStream;
		private string _Message;
		private int _CurrentWriteLength;
		private int _RetryCount;
		private System.Windows.Threading.Dispatcher _UIDispatcher;


		public FileSender(System.Windows.Threading.Dispatcher controlDispatcher)
		{
			FilePosition = 0;
			Steps = 0;
			CurrentStep = 0;
			FileLength = 0;
			ChunkRetryAttempts = 4;
			_CurrentWriteLength = 0;
			Token = Guid.Empty;
			_UIDispatcher = controlDispatcher;

			//default
			UriBuilder builder = new UriBuilder(Utility.BaseUrl);
			builder.Path = builder.Path.TrimEnd('/') + "/Services/FileReceiver.ashx";
			this.Uri = builder.Uri;
		}

		public void Send(FileInfo file)
		{
			_File = file;
			_FileStream = _File.OpenRead();
			_RetryCount = 0;
			this.FileLength = _File.Length;
			this.FilePosition = 0;
			this.Steps = (int)Math.Ceiling((double)this.FileLength / (double)Utility.chunkSize);
			this.CurrentStep = 0;
			SendNextChunk();
		}

		void SendNextChunk()
		{
			_RetryCount++;
			byte[] buffer = new byte[Utility.chunkSize];

			if (this.FilePosition != _FileStream.Position)
				_FileStream.Seek(this.FilePosition, SeekOrigin.Begin);

			_CurrentWriteLength = _FileStream.Read(buffer, 0, Utility.chunkSize);

			if (_CurrentWriteLength != Utility.chunkSize && _CurrentWriteLength > 0)
			{
				//resize buffer  to read size. 
				byte[] new_buffer = new byte[_CurrentWriteLength];
				for (int ix = 0; ix < _CurrentWriteLength; ix++)
				{
					new_buffer[ix] = buffer[ix];
				}
				buffer = null;
				buffer = new_buffer;
			}

			ChunkUploadRequest chunk_request = new ChunkUploadRequest();
			chunk_request.Chunk = buffer;
			chunk_request.Hash = Utility.GetSHA256Hash(buffer);
			chunk_request.Token = this.Token;
			_Message = Utility.SerializeXml(chunk_request);

			BeginSendRequest();
		}
		void FinishUpload()
		{
			//send the finish request. 
			FinishRequest req = new FinishRequest();
			req.Extension = Path.GetExtension(_File.Name);

			//reset filestream for full hash. 
			if (_FileStream != null)
				_FileStream.Seek(0, SeekOrigin.Begin);

			req.FullHash = Utility.GetSHA256Hash(_FileStream);

			//close filestream. 
			_FileStream.Dispose();
			req.Token = this.Token;

			_Message = Utility.SerializeXml(req);
			this.BeginSendRequest();
		}

		//http stuff. 
		void BeginSendRequest()
		{
			WebRequest req = HttpWebRequest.Create(this.Uri);
			req.Method = "POST";
			req.BeginGetRequestStream(WriteToRequestStream, req);
		}
		void WriteToRequestStream(IAsyncResult result)
		{
			WebRequest req = (WebRequest)result.AsyncState;
			Stream post_data = req.EndGetRequestStream(result);

			byte[] request_buffer = Encoding.UTF8.GetBytes(_Message);
			_Message = null;

			post_data.Write(request_buffer, 0, request_buffer.Length);
			post_data.Close();
			req.BeginGetResponse(ReadResponseStream, req);
		}
		void ReadResponseStream(IAsyncResult result)
		{
			try
			{
				WebRequest req = (WebRequest)result.AsyncState;
				WebResponse resp = req.EndGetResponse(result);
				Stream s = resp.GetResponseStream();

				StreamReader r = new StreamReader(s, Encoding.UTF8);

				string xml = r.ReadToEnd();
				string top = Utility.GetTopElement(xml);
				switch (top.Trim().ToLower())
				{
					case "uploadresponse":
						ProcessUploadResponse(xml);
						return;
					case "finishresponse":
						ProcessFinishResponse(xml);
						return;
				}

			}
			catch (WebException ex)
			{
				OnCompleted(ex);
			}
		}
		void ProcessFinishResponse(string xml)
		{
			FinishResponse resp = Utility.DeserializeXml(typeof(FinishResponse), xml) as FinishResponse;
			if (resp != null)
			{
				if (resp.Status == Enums.ResponsStatus.Success)
				{
					OnProgressChanged();
					OnCompleted(SendStatus.Success, resp.NewFilename, null);
				}
				else
					OnCompleted(SendStatus.Failed, null, resp.Message);
			}
			else
			{
				OnCompleted(SendStatus.Failed, null, "Invalid response from server");
			}
		}
		void ProcessUploadResponse(string xml)
		{
			UploadResponse resp = Utility.DeserializeXml(typeof(UploadResponse), xml) as UploadResponse;
			if (resp != null)
			{
				if (resp.Status == Enums.ResponsStatus.Success)
				{
					this.FilePosition += _CurrentWriteLength; //indicates that the last chunk was success. 
					this.Token = resp.Token;
					_RetryCount = 0;
					this.CurrentStep++;
					OnProgressChanged();

					if (this.FilePosition >= this.FileLength)//done... call finish. 
						FinishUpload();
					else
						SendNextChunk();
				}
				else if (resp.Status == Enums.ResponsStatus.Fail || resp.Status == Enums.ResponsStatus.FailFullHashCheck)
				{
					if (resp.Status == Enums.ResponsStatus.Fail)
						OnCompleted(SendStatus.Failed, null, "Failed: " + resp.Message);
					else
						OnCompleted(SendStatus.Failed, null, "Failed: Invalid file hash. Please resend file.");
				}
				else
				{
					if (_RetryCount > ChunkRetryAttempts)
						OnCompleted(SendStatus.Failed, null, "Uploaded attempt failed. Please try again later.");
					else
						SendNextChunk();
				}
			}
			else
			{
				OnCompleted(SendStatus.Failed, null, "Invalid response from server.");
			}
		}

		//events
		void OnProgressChanged()
		{
			if (_ProgressChanged != null)
			{
				Delegate d = new FileSenderProgressChangedEventHandler(_ProgressChanged);
				this._UIDispatcher.BeginInvoke(d, null, new FileSenderProgressChangedEventArgs(this.CurrentStep, this.Steps));
			}
		}
		void OnCompleted(Exception ex)
		{
			if (_Completed != null)
			{
				Delegate d = new FileSenderCompletedEventHandler(_Completed);

				FileSenderCompletedEventArgs e = new FileSenderCompletedEventArgs();
				e.Error = ex;
				e.Status = SendStatus.Failed;
				e.Message = ex.Message;

				this._UIDispatcher.BeginInvoke(d, null, e);
			}
		}
		void OnCompleted(SendStatus status, string newFilename, string message)
		{
			if (_Completed != null)
			{
				Delegate d = new FileSenderCompletedEventHandler(_Completed);

				FileSenderCompletedEventArgs e = new FileSenderCompletedEventArgs();
				e.NewFilename = newFilename;
				e.Status = status;
				e.Message = message;

				this._UIDispatcher.BeginInvoke(d, null, e);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (_FileStream != null)
				_FileStream.Dispose();
		}

		#endregion
	}
}
