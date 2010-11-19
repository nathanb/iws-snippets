using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace UploaderSL.Web.Services
{
	// NOTE: If you change the interface name "IFileReceiver" here, you must also update the reference to "IFileReceiver" in Web.config.
	[ServiceContract]
	public interface IFileReceiver
	{
		[OperationContract]
		UploadResponse BeginUpload(ChunkUploadRequest request);
		[OperationContract]
		UploadResponse ContinueUpload(ChunkUploadRequest request);
		[OperationContract]
		FinishResponse FinishUpload(FinishRequest request);
	}
}
