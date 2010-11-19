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

namespace Uploader.ClientHttp
{
	public partial class ListItem : UserControl
	{
		private FileSender _Sender;
		public ListItem()
		{
			InitializeComponent();
			_Sender = new FileSender(this.Dispatcher);
			_Sender.ProgressChanged += new FileSenderProgressChangedEventHandler(_Sender_ProgressChanged);
			_Sender.Completed += new FileSenderCompletedEventHandler(_Sender_Completed);
		}
		~ListItem()
		{
			//cleanup
			if (_Sender != null)
				_Sender.Dispose();
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

		public void Send()
		{
			_Sender.Send(this.File);
		}

		void _Sender_Completed(object sender, FileSenderCompletedEventArgs e)
		{
			if (e.Status == FileSender.SendStatus.Success)
				uxText.Text = string.Format("{0} transferred successfully. New filename is {1}.", System.IO.Path.GetFileName(_File.Name), e.NewFilename);
			else
			{
				uxText.Text = e.Message;
			}
		}
		void _Sender_ProgressChanged(object sender, FileSenderProgressChangedEventArgs e)
		{
			this.uxProgress.Maximum = e.TotalSteps;
			this.uxProgress.Value = e.CurrentStep;
		}
	}
}
