using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace PDTUtils.MVVM
{
	class MainWindowViewModel
	{
		public ObservableCollection<int> dummy;

		public MainWindowViewModel()
		{
			dummy = new ObservableCollection<int>();
		}


	}
}
