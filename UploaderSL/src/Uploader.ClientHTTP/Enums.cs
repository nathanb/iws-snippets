using System;

namespace Uploader.ClientHttp
{
	public class Enums
	{
		public enum ResponsStatus
		{
			NotSet = 0,
			Success = 1,
			Fail = 20,
			FailFullHashCheck = 21,
			FailChunkHashCheck = 22,
			FailAuthentication = 23,
			FailTracker = 24
		}
	}
}
