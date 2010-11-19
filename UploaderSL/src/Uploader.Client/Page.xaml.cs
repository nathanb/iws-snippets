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
	public partial class Page : UserControl
	{
		public Page()
		{
			InitializeComponent();

			this.uxFiles.Items.Clear();
		}

		private void uxSend_Click(object sender, RoutedEventArgs e)
		{
			foreach (object row in uxFiles.Items)
			{
				ListItem item = row as ListItem;
				if (item != null)
					item.Send();
			}
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{

		}

		private void uxBrowse_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog of = new OpenFileDialog();
			of.Filter = "All Files(*.*)|*.*";
			of.Multiselect = true;
			if (of.ShowDialog() ?? false)
			{
				foreach (FileInfo info in of.Files)
				{
					ListItem item = new ListItem();
					item.File = info;
					uxFiles.Items.Add(item);
				}
			}
		}
	}
}
