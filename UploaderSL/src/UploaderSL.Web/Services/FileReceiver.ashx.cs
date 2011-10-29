using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace UploaderSL.Web.Services
{
	/// <summary>
	/// Summary description for $codebehindclassname$
	/// </summary>
	public class FileReceiverHandler : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			ResponseBase response = null;
			if (context.Request.InputStream.Length < 10485760) //10MB
			{
				try
				{
					StreamReader reader = new StreamReader(context.Request.InputStream, Encoding.UTF8);
					string xml = reader.ReadToEnd();

					string top_element = Utility.GetTopElement(xml);

					switch (top_element.ToLower())
					{
						case "chunkuploadrequest":
							ChunkUploadRequest data = Utility.Deserialize(typeof(ChunkUploadRequest), xml) as ChunkUploadRequest;
							response = ProcessRequest(data);
							break;
						case "finishrequest":
							FinishRequest finish = Utility.Deserialize(typeof(FinishRequest), xml) as FinishRequest;
							response = ProcessRequest(finish);
							break;
					}

				}
				catch (Exception ex)
				{
					response = new ResponseBase();
					response.Status = Enums.ResponsStatus.Fail;
					response.Message = "Internal server error. \n\n" + ex.ToString() ;

					//log the message.
				}
			}

			if (response == null)
			{
				response = new ResponseBase();
				response.Status = Enums.ResponsStatus.Fail;
				response.Message = "Internal server error.";
			}

			string xml_resp = Utility.Serialize(response, Encoding.UTF8);

			context.Response.ContentType = "text/xml";
			context.Response.Write(xml_resp);
			context.ApplicationInstance.CompleteRequest();
		}

		ResponseBase ProcessRequest(FinishRequest request)
		{
			FinishResponse resp = new FinishResponse();

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

			return resp;
		}

		ResponseBase ProcessRequest(ChunkUploadRequest request)
		{
			UploadResponse resp = new UploadResponse();

			UploadProcessor proc = null;
			if (request.Token == Guid.Empty)
				proc = new UploadProcessor();
			else
				proc = new UploadProcessor(request.Token);

			if (proc.ProcessChunk(request.Chunk, request.Hash))
			{
				resp.Status = Enums.ResponsStatus.Success;
				resp.Token = proc.Token;
			}
			else
			{
				resp.Status = Enums.ResponsStatus.Fail;
				resp.Message = "chunk failed.";
			}

			return resp;
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}
}
