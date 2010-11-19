using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Uploader.Client
{
	public class Items :
		 System.Collections.ObjectModel.ObservableCollection<string>
	{
		public Items()
		{
			Add("Item 1");
			Add("Item 2");
			Add("Item 3");
			Add("Item 4");
			Add("Item 5");
		}
	}

}
