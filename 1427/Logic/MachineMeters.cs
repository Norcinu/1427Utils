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
		public ObservableCollection<MeterDescription> _meterDesc = new ObservableCollection<MeterDescription>();
        public ObservableCollection<MeterDescription> MeterDesc { get { return _meterDesc; } }
        
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
            if (_meterDesc.Count > 0)
                _meterDesc.RemoveAll();

            _meterDesc.Add(new MeterDescription("Cash In", BoLib.getCashIn(BoLib.useMoneyInType(0)).ToString()));
            _meterDesc.Add(new MeterDescription("Cash Out", BoLib.getCashOut(BoLib.useMoneyOutType(0)).ToString()));
            _meterDesc.Add(new MeterDescription("Notes In", BoLib.getNotesIn(BoLib.useMoneyInType(0)).ToString()));
            _meterDesc.Add(new MeterDescription("Notes Out", BoLib.getNotesOut(BoLib.useMoneyOutType(0)).ToString()));
            _meterDesc.Add(new MeterDescription("Refill", BoLib.getRefillValue(BoLib.useRefillType(0)).ToString()));
            
            uint won = 0;
            uint bet = 0;
            
            for (uint i = 1; i <= BoLib.getNumberOfGames(); i++)
            {
                bet += (uint)BoLib.getGamePerformanceMeter(i, (uint)GamePerformance.GamePlaySt); //wriong
                won += (uint)BoLib.getGamePerformanceMeter(i, (uint)GamePerformance.GameWonSt);
            }

            _meterDesc.Add(new MeterDescription("Fischas Bet", bet.ToString()));
            _meterDesc.Add(new MeterDescription("Fischas Win", won.ToString()));
            //_meterDesc.Add(new MeterDescription("Fischas Bet", BoLib.getVtp(BoLib.useVtpMeter(0)).ToString()));
            //_meterDesc.Add(new MeterDescription("Fischas Win", BoLib.getWon(BoLib.useWonMeter(0)).ToString()));
            _meterDesc.Add(new MeterDescription("Hand Pay", BoLib.getHandPay(BoLib.useHandPayMeter(0)).ToString()));
            _meterDesc.Add(new MeterDescription("Ticket Out", BoLib.getTicketsPay(BoLib.useTicketsMeter(0)).ToString()));
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
            if (_meterDesc.Count > 0)
                _meterDesc.RemoveAll();

			_meterDesc.Add(new MeterDescription("Cash In", BoLib.getCashIn(BoLib.useMoneyInType(1)).ToString()));
			_meterDesc.Add(new MeterDescription("Cash Out", BoLib.getCashOut(BoLib.useMoneyOutType(1)).ToString()));
			_meterDesc.Add(new MeterDescription("Notes In", BoLib.getNotesIn(BoLib.useMoneyInType(1)).ToString()));
			_meterDesc.Add(new MeterDescription("Notes Out", BoLib.getNotesOut(BoLib.useMoneyOutType(1)).ToString()));
			_meterDesc.Add(new MeterDescription("Refill", BoLib.getRefillValue(BoLib.useRefillType(1)).ToString()));
			
            uint won = 0;
            uint bet = 0;
            
            for (uint i = 1; i <= BoLib.getNumberOfGames(); i++)
            {
                bet += (uint)BoLib.getGamePerformanceMeter(i, (uint)GamePerformance.GamePlayLt);
                won += (uint)BoLib.getGamePerformanceMeter(i, (uint)GamePerformance.GameWonLt);
            }

            _meterDesc.Add(new MeterDescription("Fischas Bet", bet.ToString()));
            _meterDesc.Add(new MeterDescription("Fischas Win", won.ToString()));

            //_meterDesc.Add(new MeterDescription("Fischas Bet", BoLib.getVtp(BoLib.useVtpMeter(1)).ToString()));
			//_meterDesc.Add(new MeterDescription("Fischas Win", BoLib.getWon(BoLib.useWonMeter(1)).ToString()));
			_meterDesc.Add(new MeterDescription("Hand Pay", BoLib.getHandPay(BoLib.useHandPayMeter(1)).ToString()));
			_meterDesc.Add(new MeterDescription("Ticket Out", BoLib.getTicketsPay(BoLib.useTicketsMeter(1)).ToString()));
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
            
            _meterDesc.Add(new MeterDescription("TicketIn", ti[1]));
            _meterDesc.Add(new MeterDescription("TicketOut", to[1]));
            OnPropertyChanged("TitoMeter");
        }
    }
}

