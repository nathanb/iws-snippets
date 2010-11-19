using System;

namespace Uploader.ClientHttp
{
	public delegate void FileSenderProgressChangedEventHandler(object sender, FileSenderProgressChangedEventArgs e);

	public class FileSenderProgressChangedEventArgs : EventArgs
	{
		public int CurrentStep { get; set; }
		public int TotalSteps { get; set; }

		public FileSenderProgressChangedEventArgs()
			: base()
		{
		}

		public FileSenderProgressChangedEventArgs(int step, int total)
			: base()
		{
			this.CurrentStep = step;
			this.TotalSteps = total;
		}
	}

	public delegate void FileSenderCompletedEventHandler(object sender, FileSenderCompletedEventArgs e);

	public class FileSenderCompletedEventArgs : EventArgs
	{
		public Exception Error { get; set; }
		public string NewFilename { get; set; }
		public FileSender.SendStatus Status { get; set; }
        public string Message { get; set; }

		public FileSenderCompletedEventArgs()
			: base()
		{
		}
	}
}
