using System.Collections.ObjectModel;
using System.ComponentModel;
using PDTUtils.Logic;
using PDTUtils.Native;
using PDTUtils.Properties;

namespace PDTUtils
{
	public class MeterDescription
	{
		public string Key { get; set; }
		public string Value { get; set; }
        
		public MeterDescription() { }
		public MeterDescription(string k, string v)
		{
			this.Key = k;
			this.Value = v;
		}
	}
    
	abstract public class MachineMeters : INotifyPropertyChanged
	{
		public ObservableCollection<MeterDescription> m_meterDesc = new ObservableCollection<MeterDescription>();
        public ObservableCollection<MeterDescription> MeterDesc { get { return m_meterDesc; } }

		public MachineMeters()
		{
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public void OnPropertyChanged(string name)
		{
			if (this.PropertyChanged != null)
				this.PropertyChanged(this, new PropertyChangedEventArgs(name));
		}
        
		abstract public void ReadMeter();
	}
    
	public class ShortTermMeters : MachineMeters
	{
		public ShortTermMeters()
		{
		}

		public override void ReadMeter()
		{
			m_meterDesc.Add(new MeterDescription("Cash In", BoLib.getCashIn(BoLib.useMoneyInType(0)).ToString()));
			m_meterDesc.Add(new MeterDescription("Cash Out", BoLib.getCashOut(BoLib.useMoneyOutType(0)).ToString()));
			m_meterDesc.Add(new MeterDescription("Notes In", BoLib.getNotesIn(BoLib.useMoneyInType(0)).ToString()));
			m_meterDesc.Add(new MeterDescription("Notes Out", BoLib.getNotesOut(BoLib.useMoneyOutType(0)).ToString()));
			m_meterDesc.Add(new MeterDescription("Refill", BoLib.getRefillValue(BoLib.useRefillType(0)).ToString()));
			m_meterDesc.Add(new MeterDescription("VTP", BoLib.getVtp(BoLib.useVtpMeter(0)).ToString()));
			m_meterDesc.Add(new MeterDescription("Won", BoLib.getWon(BoLib.useWonMeter(0)).ToString()));
			m_meterDesc.Add(new MeterDescription("Hand Pay", BoLib.getHandPay(BoLib.useHandPayMeter(0)).ToString()));
            m_meterDesc.Add(new MeterDescription("Ticket Out", BoLib.getTicketsPay(BoLib.useTicketsMeter(0)).ToString()));
			this.OnPropertyChanged("ShortTerm");
		}
	}
    
    public class LongTermMeters : MachineMeters
	{
		public LongTermMeters()
		{
		}
        
		public override void ReadMeter()
		{
			m_meterDesc.Add(new MeterDescription("Cash In", BoLib.getCashIn(BoLib.useMoneyInType(1)).ToString()));
			m_meterDesc.Add(new MeterDescription("Cash Out", BoLib.getCashOut(BoLib.useMoneyOutType(1)).ToString()));
			m_meterDesc.Add(new MeterDescription("Notes In", BoLib.getNotesIn(BoLib.useMoneyInType(1)).ToString()));
			m_meterDesc.Add(new MeterDescription("Notes Out", BoLib.getNotesOut(BoLib.useMoneyOutType(1)).ToString()));
			m_meterDesc.Add(new MeterDescription("Refill", BoLib.getRefillValue(BoLib.useRefillType(1)).ToString()));
			m_meterDesc.Add(new MeterDescription("VTP", BoLib.getVtp(BoLib.useVtpMeter(1)).ToString()));
			m_meterDesc.Add(new MeterDescription("Won", BoLib.getWon(BoLib.useWonMeter(1)).ToString()));
			m_meterDesc.Add(new MeterDescription("Hand Pay", BoLib.getHandPay(BoLib.useHandPayMeter(1)).ToString()));
			m_meterDesc.Add(new MeterDescription("Ticket Out", BoLib.getTicketsPay(BoLib.useTicketsMeter(1)).ToString()));
			this.OnPropertyChanged("LongTerm");
		}
    }
    
    public class TitoMeters : MachineMeters
    {
        public TitoMeters()
        {
        }

        public override void ReadMeter()
        {
            string[] ticketsIn;
            string[] ticketsOut;
            
            IniFileUtility.GetIniProfileSection(out ticketsIn, "TicketsIn", @Resources.tito_log);
            IniFileUtility.GetIniProfileSection(out ticketsOut, "TicketsOut", @Resources.tito_log);

            var ti = ticketsIn[0].Split("=".ToCharArray());
            var to = ticketsOut[0].Split("=".ToCharArray());

            m_meterDesc.Add(new MeterDescription("TicketIn", ti[1]));
            m_meterDesc.Add(new MeterDescription("TicketOut", to[1]));
            this.OnPropertyChanged("TitoMeter");
        }
    }
}
