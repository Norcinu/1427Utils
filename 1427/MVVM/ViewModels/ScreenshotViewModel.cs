using System.IO;
using System.Windows.Input;
using System.Collections.ObjectModel;
using PDTUtils.MVVM.Models;

namespace PDTUtils.MVVM.ViewModels
{
    class ScreenshotViewModel : ObservableObject
    {
        public int CurrentImageID { get; set; }
        public int NumberOfImages { get; set; }

        ObservableCollection<ScreenshotModel> Files = new ObservableCollection<ScreenshotModel>();

        public ScreenshotViewModel()
        {
            CurrentImageID = 0;
            NumberOfImages = 0; 
        }
        
        public ICommand LoadImage { get { return new DelegateCommand(o => DoLoadImages()); } }
        void DoLoadImages()
        {
            var files = Directory.GetFiles(@"D:\screenshots");
            foreach (var str in files)
            {
                Files.Add(new ScreenshotModel(CurrentImageID++, str));
                NumberOfImages++;
            }
        }
        
        public ICommand Forward { get { return new DelegateCommand(o => DoForwardImage()); } }
        void DoForwardImage()
        {
            if (CurrentImageID < NumberOfImages)
                CurrentImageID++;
        }
                
        public ICommand Back { get { return new DelegateCommand(o => DoBackwardImage()); } }
        void DoBackwardImage()
        {
            if (CurrentImageID > 0)
                --CurrentImageID;
        }
        
        public ICommand Front { get { return new DelegateCommand(o => DoFront()); } }
        void DoFront()
        {
            CurrentImageID = 0;
        }

        public ICommand End { get { return new DelegateCommand(o => DoEnd()); } }
        void DoEnd() 
        {
            CurrentImageID = NumberOfImages;
        }
    }
}
