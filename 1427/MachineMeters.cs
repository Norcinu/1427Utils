using System.ComponentModel;
using PDTUtils.Native;

namespace PDTUtils
{
	public class ShortTermMeter
	{
		public uint CashIn { get; set; }
		public uint CashOut { get; set; }
		public uint NotesIn { get; set; }
		public uint NotesOut { get; set; }
		public uint Refill { get; set; }
		public uint Vtp { get; set; }
		public uint Won { get; set; }
		public uint HandPay { get; set; }
		public uint TicketOut { get; set; }
	}


	public class MachineMeters : INotifyPropertyChanged
	{
		public ShortTermMeter LCDShortTerm { get; set; }

		public MachineMeters()
		{

		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		void ReadMeters()
		{
//			LCDShortTerm.CashIn=BoLib.
		}
	}
}
