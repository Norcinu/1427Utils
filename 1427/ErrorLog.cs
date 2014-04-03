using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace PDTUtils
{
	public class ErrorLog : ObservableCollection<ErrorLogData>
	{
		public ErrorLog() : base()
		{
			
		}
	}

	public class ErrorLogData
	{
		public string Date { get; set; }
		public string ErrCode { get; set; }
		public string Description { get; set; }
	}
}
