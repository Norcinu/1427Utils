using System.Collections.ObjectModel;
using System.Windows.Input;
using PDTUtils.Native;

namespace PDTUtils.MVVM.ViewModels
{
    struct CpuEvents
    {
        string TimeStamp { get; set; }
        string Event { get; set; }
    }

    class CpuCaseOpenViewModel : ObservableObject
    {
        bool _showListView = false;
        readonly int _numberOfEvents = 255;
        //ObservableCollection<CpuEvents> _eventList = new ObservableCollection<CpuEvents>();
        //public ObservableCollection<CpuEvents> EventList { get { return _eventList; } }
        ObservableCollection<string> _eventList = new ObservableCollection<string>();
        public ObservableCollection<string> EventList { get { return _eventList; } }
        public bool ShowListView
        {
            get { return _showListView; }
            set
            {
                _showListView = value;
                RaisePropertyChangedEvent("ShowListView");
            }
        }
        
        public CpuCaseOpenViewModel()
        {
            BoLib.setUtilRequestBitState((int)UtilBits.ReadCpuEventsBit);
        }
        
        public ICommand LoadEvents { get { return new DelegateCommand(o => DoLoadEvents()); } }
        void DoLoadEvents()
        {
            if (_eventList.Count > 0)
                _eventList.Clear();

            ShowListView = true;

            for (int i = 0; i < _numberOfEvents; i++)
            {
                char[] _line = new char[1024];
                NativeWinApi.GetPrivateProfileString("EventLog", (i + 1).ToString(), "", _line, _line.Length, 
                    Properties.Resources.cpu_event_log);
                _eventList.Add(new string(_line).Trim("\0".ToCharArray()));
            }
            RaisePropertyChangedEvent("EventList");
        }
    }
}
