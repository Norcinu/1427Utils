using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Input;
using PDTUtils.Native;
using PDTUtils.Properties;
using Timer = System.Timers.Timer;

namespace PDTUtils.MVVM.ViewModels
{
    class NoteAdminViewModel : ObservableObject
    {
        bool _isSpanish = false;
        public bool HasRecycler { get; set; }
        public string RecyclerMessage { get; set; }
        public string NoteOne { get; set; }
        public string NoteTwo { get; set; }
        public string RecyclerValue { get; set; }

        Timer _recycleRunChecker = new Timer() { Interval = 1000, Enabled = false };
        
        public NoteAdminViewModel()
        {
            try
            {
                _isSpanish = BoLib.getCountryCode() == BoLib.getSpainCountryCode();
                Thread.CurrentThread.CurrentUICulture = _isSpanish ? new CultureInfo("es-ES") : new CultureInfo("en-GB");
                
                if (BoLib.getBnvType() == 5)
                {
                    HasRecycler = true;
                    if (BoLib.getRecyclerChannel() == 3)
                        RecyclerMessage = _isSpanish ? "€20 NOTE TO BE RECYCLED" : "£20 NOTE TO BE RECYCLED";
                    else
                        RecyclerMessage = _isSpanish ? "€10 NOTE TO BE RECYCLED" : "£10 TO BE NOTE RECYCLED";
                }
                else
                {
                    HasRecycler = false;
                    RecyclerMessage = "NO RECYCLER";
                }
               

                NoteOne = _isSpanish ? "€10" : "£10";
                NoteTwo = _isSpanish ? "€20" : "£20";

                Thread.Sleep(2000);
                RecyclerValue = BoLib.getRecyclerFloatValue().ToString();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            RaisePropertyChangedEvent("HasRecycler");
            RaisePropertyChangedEvent("RecyclerMessage");
            RaisePropertyChangedEvent("NoteOne");
            RaisePropertyChangedEvent("NoteTwo");
            RaisePropertyChangedEvent("RecyclerValue");
        }

        // rename this to set note or something.
        public ICommand Recycle { get { return new DelegateCommand(DoRecycleNote); } }
        void DoRecycleNote(object o)
        {
            var noteType = o as string;

            if (BoLib.getBnvType() != 5) return;
            
            var channel = (noteType == "10") ? "2" : "3";
            BoLib.shellSendRecycleNote();
            NativeWinApi.WritePrivateProfileString("Config", "RecyclerChannel", channel, Resources.birth_cert);
            PDTUtils.Logic.IniFileUtility.HashFile(Resources.birth_cert);
            RecyclerMessage = (noteType == "10") ? NoteOne + " NOTE TO BE RECYCLED" : NoteTwo + " NOTE TO BE RECYCLED";
            RaisePropertyChangedEvent("RecyclerMessage");
        }

        public ICommand EmptyRecycler { get { return new DelegateCommand(o => DoEmptyRecycler()); } }
        void DoEmptyRecycler()
        {
            BoLib.shellSendEmptyRecycler();
            _recycleRunChecker.Elapsed += new System.Timers.ElapsedEventHandler(_recycleRunChecker_Elapsed);
            _recycleRunChecker.Enabled = true;

            Thread.Sleep(500);
            RecyclerValue = BoLib.getRecyclerFloatValue().ToString();
            RaisePropertyChangedEvent("RecyclerValue");
        }

        void _recycleRunChecker_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (BoLib.getOogaDeBooga())
            {
                Debug.WriteLine(BoLib.getRecyclerFloatValue().ToString());
            }
            else
            {
                _recycleRunChecker.Enabled = false;
            }
        }
        
        void Refresh()
        {
            if (_recycleRunChecker.Enabled) _recycleRunChecker.Enabled = false;

            RecyclerValue = BoLib.getRecyclerFloatValue().ToString();
            RaisePropertyChangedEvent("RecyclerValue");
        }
    }
}

