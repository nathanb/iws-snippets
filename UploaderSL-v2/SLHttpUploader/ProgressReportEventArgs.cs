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
	public class ProgressReportEventArgs : EventArgs
	{
		public long Current { get; set; }
		public long Total { get; set; }

		public int Percentage
		{
			get
			{
				if (Total == 0)
					return 0;

				double perc = (double)Current / (double)Total;
				return (int)(perc * 100);
			}
		}
	}
}
