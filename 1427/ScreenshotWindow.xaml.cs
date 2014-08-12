using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PDTUtils
{
    /// <summary>
    /// Interaction logic for ScreenshotWindow.xaml
    /// </summary>
    public partial class ScreenshotWindow : Window
    {
        List<System.Drawing.Image> images = new List<System.Drawing.Image>();
        public ScreenshotWindow()
        {
            InitializeComponent();
            DirectoryInfo di = new DirectoryInfo(@"D:\screenshots");
            foreach (string path in Directory.GetFiles(@"D:\screenshots", "*.jpg", SearchOption.TopDirectoryOnly))
            {
                images.Add(System.Drawing.Image.FromFile(path));
            }

            SetImageSource();
        }

        private void SetImageSource()
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
            images[0].Save(ms, ImageFormat.Jpeg);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            image1.Source = bi;
        }
    }
}
