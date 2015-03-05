using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using PDTUtils.MVVM;

namespace SerialExtractionTool.ViewModels
{
    class MenuScreenViewModel : ObservableObject
    {
        public MenuScreenViewModel()
        {
            IsEnabled = true;
            ShutDown = false;
            
            RaisePropertyChangedEvent("IsEnabled");
            RaisePropertyChangedEvent("ShutDown");
        }

        public ICommand ShutdownProgram
        {
            get { return new DelegateCommand(o => DoShutdown()); }
        }

        void DoShutdown()
        {
            Application.Current.Shutdown();
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        public bool IsEnabled { get; set; }
        public bool Shutdown { get; set; }
    }
}
