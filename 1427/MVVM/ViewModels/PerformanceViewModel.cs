using System.Collections.ObjectModel;
using PDTUtils.MVVM.Models;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    class PerformanceViewModel : ObservableObject
    {
        LongTermMeters _longTerm = new LongTermMeters();
        ShortTermMeters _shortTerm = new ShortTermMeters();
        TitoMeters _titoMeters = new TitoMeters(); 
        ObservableCollection<CashReconiliation> _cashRecon = new ObservableCollection<CashReconiliation>();


#region Properties
        public LongTermMeters LongTerm { get { return _longTerm; } }
        public ShortTermMeters ShortTerm { get { return _shortTerm; } }
        public TitoMeters TitoMeter { get { return _titoMeters; } }
        public ObservableCollection<CashReconiliation> CashRecon { get { return _cashRecon; } }
#endregion
        
        public PerformanceViewModel()
        {
            _longTerm.ReadMeter();
            _shortTerm.ReadMeter();
            _titoMeters.ReadMeter();

            this.ReadLCD();

            this.RaisePropertyChangedEvent("LongTerm");
            this.RaisePropertyChangedEvent("ShortTerm");
            this.RaisePropertyChangedEvent("TitoMeters");
        }

        public void ReadLCD()
        {
            string[] note_headers = new string[] { "£50 NOTES IN", "£20 NOTES IN", "£10 NOTES IN", "£5 NOTES IN" };
            string[] cash_headers = new string[] { "TOTAL CASH IN", "TOTAL CASH OUT", "TOTAL" };
            string[] coin_headers = new string[] { "£2 COINS IN", "£1 COINS IN", "50p COINS IN", "20p COINS IN" };

            _cashRecon.Add(new CashReconiliation("STAKE IN", 
                                                 BoLib.useStackInMeter(0).ToString(), 
                                                 BoLib.useStackInMeter(1).ToString()));
            
            for (int i = 0; i <= 3; i++)
            {
                _cashRecon.Add(new CashReconiliation(note_headers[i], 
                                                     BoLib.getNotesIn(0).ToString(), 
                                                     BoLib.getNotesIn(1).ToString()));
            }

            for (int i = 0; i <= 1; i++)
            {
                _cashRecon.Add(new CashReconiliation(cash_headers[i],
                                                     BoLib.getCoinsIn(0).ToString(),
                                                     BoLib.getCoinsIn(1).ToString()));
            }

            this.RaisePropertyChangedEvent("CashRecon");
        }
    }
}
