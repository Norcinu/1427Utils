using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace PDTUtils
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        MainWindow mainWindow = new MainWindow();
        int Counter { get; set; }
        public LoadingWindow()
        {
            InitializeComponent();
            Counter = 0;
        }
        
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            
            worker.RunWorkerAsync();
        }
        //if we weea
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (Counter < 2000)
            {
                Counter += 100;
                Thread.Sleep(100);
            }

            if (mainWindow.FullyLoaded)// && Counter >= 2000)
            {
                try
                {
                    this.Dispatcher.Invoke((DelegateWindow)ShowMainWindow, new object[] { mainWindow, this });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            else
            {
                Counter += 100;
                Thread.Sleep(100);
            }
        }
        
        public delegate void DelegateWindow(Window window, Window current);
        public void ShowMainWindow(Window window, Window current)
        {
            current.Close();
            window.Show();
        }
        
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbStatus.Value = e.ProgressPercentage;
        }
    }
}
