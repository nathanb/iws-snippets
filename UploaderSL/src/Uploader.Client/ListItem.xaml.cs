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

namespace Uploader.Client
{
	public partial class ListItem : UserControl
	{
		public ListItem()
		{
			InitializeComponent();
			FilePosition = 0;
		}

		FileInfo _File;
		public FileInfo File
		{
			get { return _File; }
			set
			{
				_File = value;
				this.uxText.Text = _File.Name;
			}
		}
		private long FilePosition;
		private long FileLength;
		private int CurrentStep = 0;
		private int Steps = 0;

		public void Send()
		{
			try
			{
				//send first chunk, start upload. 

				FileReceiver.FileReceiverClient client = new Uploader.Client.FileReceiver.FileReceiverClient();
				client.Endpoint.Address = new System.ServiceModel.EndpointAddress(Utility.BaseUrl + "Services/FileReceiver.svc");
				client.BeginUploadCompleted += new EventHandler<Uploader.Client.FileReceiver.BeginUploadCompletedEventArgs>(client_BeginUploadCompleted);
				FileReceiver.ChunkUploadRequest req = new Uploader.Client.FileReceiver.ChunkUploadRequest();
				using (FileStream fs = this.File.OpenRead())
				{
					//get full hash first. 
					this.FileLength = fs.Length;

					//setup progress bar. 
					this.Steps = (int)(this.FileLength / (long)Utility.chunkSize);
					this.uxProgress.Minimum = 0;
					this.uxProgress.Maximum = this.Steps;

					int read = 0;
					byte[] buffer = null;

					if (fs.Length <= Utility.chunkSize)
						buffer = new byte[(int)fs.Length];
					else
						buffer = new byte[Utility.chunkSize];

					read = fs.Read(buffer, 0, Utility.chunkSize);

					this.FilePosition += read;

					req.Chunk = buffer;
					req.ChunkSize = buffer.Length;
					req.Hash = Utility.GetSHA256Hash(buffer);

					client.BeginUploadAsync(req);
				}
			}
			catch (Exception ex)
			{
				//show error. 
				this.uxText.Text = ex.Message;
			}
		}
		void SendNextChunk(Guid token)
		{
			try
			{
				FileReceiver.FileReceiverClient client = new Uploader.Client.FileReceiver.FileReceiverClient();
				client.Endpoint.Address = new System.ServiceModel.EndpointAddress(Utility.BaseUrl + "Services/FileReceiver.svc");
				client.ContinueUploadCompleted += new EventHandler<Uploader.Client.FileReceiver.ContinueUploadCompletedEventArgs>(client_ContinueUploadCompleted);

				FileReceiver.ChunkUploadRequest req = new Uploader.Client.FileReceiver.ChunkUploadRequest();
				using (FileStream fs = this.File.OpenRead())
				{
					int read = 0;
					byte[] buffer = null;
					int readSize = Utility.chunkSize;

					long diff = this.FileLength - this.FilePosition;
					if (diff < Utility.chunkSize)
						readSize = (int)diff;

					buffer = new byte[readSize];

					fs.Seek(this.FilePosition, SeekOrigin.Begin);
					read = fs.Read(buffer, 0, readSize);

					this.FilePosition += read;

					req.ChunkSize = buffer.Length;
					req.Hash = Utility.GetSHA256Hash(buffer);
					req.Chunk = buffer;
					req.Token = token;

					client.ContinueUploadAsync(req);
				}
			}
			catch (Exception ex)
			{
				//show error. 
				this.uxText.Text = ex.Message;
			}
		}

		void client_ContinueUploadCompleted(object sender, Uploader.Client.FileReceiver.ContinueUploadCompletedEventArgs e)
		{
			if (!e.Cancelled && e.Error == null)
			{
				if (e.Result.Status == Uploader.Client.FileReceiver.EnumsResponsStatus.Success)
				{
					UpdateProgress();

					if (this.FilePosition < this.FileLength)
						SendNextChunk(e.Result.Token);
					else
					{
						//done.. 
						FinishUpload(e.Result.Token);
					}
				}
			}
			else if (e.Error != null)
			{
				this.uxText.Text = e.Error.Message;
			}
			else
			{
				this.uxText.Text = "Cancelled";
			}
		}
		void client_BeginUploadCompleted(object sender, Uploader.Client.FileReceiver.BeginUploadCompletedEventArgs e)
		{
			if (!e.Cancelled && e.Error == null)
			{
				if (e.Result.Status == Uploader.Client.FileReceiver.EnumsResponsStatus.Success)
				{
					UpdateProgress();

					if (this.FilePosition < this.FileLength)
						SendNextChunk(e.Result.Token);
					else
					{
						//finish upload. 
						FinishUpload(e.Result.Token);
					}
				}
				else
					uxText.Text = e.Result.Message;
			}
			else if (e.Error != null)
			{
				this.uxText.Text = e.Error.Message;
			}
			else
			{
				this.uxText.Text = "Cancelled";
			}
		}

		void FinishUpload(Guid token)
		{
			FileReceiver.FileReceiverClient client = new Uploader.Client.FileReceiver.FileReceiverClient();
			client.Endpoint.Address = new System.ServiceModel.EndpointAddress(Utility.BaseUrl + "Services/FileReceiver.svc");
			client.FinishUploadCompleted += new EventHandler<Uploader.Client.FileReceiver.FinishUploadCompletedEventArgs>(client_FinishUploadCompleted);

			FileReceiver.FinishRequest req = new Uploader.Client.FileReceiver.FinishRequest();
			using (FileStream fs = this.File.OpenRead())
			{
				req.Extension = this.File.Extension;
				req.FullHash = Utility.GetSHA256Hash(fs);
			}
			req.Token = token;

			client.FinishUploadAsync(req);
		}

		void UpdateProgress()
		{
			this.CurrentStep++;
			this.uxProgress.Value = this.CurrentStep;
		}

		void client_FinishUploadCompleted(object sender, Uploader.Client.FileReceiver.FinishUploadCompletedEventArgs e)
		{
			if (!e.Cancelled && e.Error == null)
			{
				if (e.Result.Status == Uploader.Client.FileReceiver.EnumsResponsStatus.Success)
				{
					uxText.Text = "Transferred successfully.";
				}
				else
					uxText.Text = e.Result.Message;
			}
			else
			{
				if (e.Error != null)
					uxText.Text = e.Error.Message;
				else
					uxText.Text = "Cancelled";
			}
		}
	}
}
