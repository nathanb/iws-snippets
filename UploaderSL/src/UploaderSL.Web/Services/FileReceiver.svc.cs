using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.IO;

namespace UploaderSL.Web.Services
{
	// NOTE: If you change the class name "FileReceiver" here, you must also update the reference to "FileReceiver" in Web.config.
	public class FileReceiver : IFileReceiver
	{
		#region IFileReceiver Members

		public UploadResponse BeginUpload(ChunkUploadRequest request)
		{
			UploadResponse resp = new UploadResponse();
			try
			{
				//you could validate credentials here. 
				//request.Credentials

				UploadProcessor proc = new UploadProcessor(); //sets up destination and initializes the file. 

				if (proc.ProcessChunk(request.Chunk, request.Hash))
				{
					resp.Token = proc.Token;
					resp.Status = Enums.ResponsStatus.Success; //continue uploading. 
				}
				else
				{
					resp.Status = Enums.ResponsStatus.Fail;
					resp.Message = "Couldn't handle chunk.";
				}
			}
			catch (Exception ex)
			{
				resp.Status = Enums.ResponsStatus.Fail;
				resp.Message = ex.Message;
			}
			return resp;
		}

		public UploadResponse ContinueUpload(ChunkUploadRequest request)
		{
			UploadResponse resp = new UploadResponse();
			try
			{
				UploadProcessor proc = new UploadProcessor(request.Token);
				if (proc.ProcessChunk(request.Chunk, request.Hash))
				{
					resp.Status = Enums.ResponsStatus.Success;
					resp.Token = proc.Token;
				}
				else
				{
					resp.Status = Enums.ResponsStatus.Fail;
					resp.Message = "failed chunk send.";
				}
			}
			catch (Exception ex)
			{
				resp.Status = Enums.ResponsStatus.Fail;
				resp.Message = ex.Message;
			}
			return resp;
		}

		public FinishResponse FinishUpload(FinishRequest request)
		{
			FinishResponse resp = new FinishResponse();

			try
			{
				UploadProcessor proc = new UploadProcessor(request.Token);
				if (proc.Finish(request.FullHash, request.Extension))
				{
					resp.Status = Enums.ResponsStatus.Success;
					resp.NewFilename = proc.Filename;
				}
				else
				{
					resp.Status = Enums.ResponsStatus.FailFullHashCheck;
					resp.Message = "Failed wrap-up procress.";
				}
			}
			catch (Exception ex)
			{
				resp.Status = Enums.ResponsStatus.Fail;
				resp.Message = ex.Message;
			}

			return resp;
		}
		#endregion
	}
}
