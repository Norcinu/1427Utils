using System.ComponentModel;
using PDTUtils.Native;
using System.Collections.ObjectModel;

namespace PDTUtils
{
	public class Meter
	{
		public uint CoinIn { get; set; }
		public uint CoinOut { get; set; }
		public uint NotesIn { get; set; }
		public uint NotesOut { get; set; }
		public uint Refill { get; set; }
		public uint Vtp { get; set; }
		public uint Won { get; set; }
		public uint HandPay { get; set; }
		public uint TicketOut { get; set; }

		public Meter()
		{
			CoinIn = 77676;
		}
	}

	public class MachineMeters : INotifyPropertyChanged
	{
		Meter m_lcdShortTerm = new Meter();
		Meter m_lcdLongTerm = new Meter();

		public Meter LCDShortTerm { get { return m_lcdShortTerm; } }
		public Meter LCDLongTerm { get { return m_lcdLongTerm; } }

		ObservableCollection<Meter> me = new ObservableCollection<Meter>();
				
		public MachineMeters()
		{
			ReadMeters();
			me.Add(m_lcdShortTerm);
			me.Add(m_lcdLongTerm);
		}

		public ObservableCollection<Meter> Me
		{
			get { return me; }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		public void ReadMeters()
		{
			LCDShortTerm.CoinIn =  BoLib.getCoinsIn(BoLib.useMoneyInType(0));
			LCDShortTerm.CoinOut = BoLib.getCoinsOut(BoLib.useMoneyOutType(0));
			LCDShortTerm.NotesIn = BoLib.getNotesIn(BoLib.useMoneyInType(0));
			LCDShortTerm.NotesOut = BoLib.getNotesOut(BoLib.useMoneyOutType(0));
			LCDShortTerm.Refill = BoLib.getRefillValue(BoLib.useRefillType(0));
			LCDShortTerm.Vtp = BoLib.getVtp(BoLib.useVtpMeter(0));
			LCDShortTerm.Won = BoLib.getWon(BoLib.useWonMeter(0));
			LCDShortTerm.HandPay = BoLib.getHandPay(BoLib.useHandPayMeter(0));
			LCDShortTerm.TicketOut = BoLib.getTicketsPay(BoLib.useTicketsMeter(0));

			LCDLongTerm.CoinIn = BoLib.getCoinsIn(BoLib.useMoneyInType(1));
			LCDLongTerm.CoinOut = BoLib.getCoinsOut(BoLib.useMoneyOutType(1));
			LCDLongTerm.NotesIn = BoLib.getNotesIn(BoLib.useMoneyInType(1));
			LCDLongTerm.NotesOut = BoLib.getNotesOut(BoLib.useMoneyOutType(1));
			LCDLongTerm.Refill = BoLib.getRefillValue(BoLib.useRefillType(1));
			LCDLongTerm.Vtp = BoLib.getVtp(BoLib.useVtpMeter(1));
			LCDLongTerm.Won = BoLib.getWon(BoLib.useWonMeter(1));
			LCDLongTerm.HandPay = BoLib.getHandPay(BoLib.useHandPayMeter(1));
			LCDLongTerm.TicketOut = BoLib.getTicketsPay(BoLib.useTicketsMeter(1));
			
			this.OnPropertyChanged("ReadMeters");
		}
	}
}
