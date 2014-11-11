using System;
using System.Collections.Generic;
using System.Text;
using PDTUtils.Native;
using PDTUtils.Logic;

namespace PDTUtils.MVVM.ViewModels
{
    class PerformanceViewModel : ObservableObject
    {
        LongTermMeters _longTerm = new LongTermMeters();
        ShortTermMeters _shortTerm = new ShortTermMeters();
        TitoMeters _titoMeters = new TitoMeters();
        
        public PerformanceViewModel()
        {
            _longTerm.ReadMeter();
            _shortTerm.ReadMeter();
            _titoMeters.ReadMeter();
            this.RaisePropertyChangedEvent("LongTerm");
            this.RaisePropertyChangedEvent("ShortTerm");
            this.RaisePropertyChangedEvent("TitoMeters");
        }
    }
}
