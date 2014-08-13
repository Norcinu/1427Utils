using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Input;

namespace PDTUtils.Models
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ScreenshotModel
    {
        public uint CurrentIndex { get; set; }
        public uint MaxIndex { get; set; }

        public ScreenshotModel()
        {
            CurrentIndex = 0;
            MaxIndex = 0;
        }
    }
}
