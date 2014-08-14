using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using System;

namespace PDTUtils
{
    /// <summary>
    /// Interaction logic for ScreenshotWindow.xaml
    /// </summary>
    public partial class ScreenshotWindow : Window
    {
        List<System.Drawing.Image> images = new List<System.Drawing.Image>();
        int currentImage = 0;
        int maxImages = 0;

        public ScreenshotWindow()
        {
            InitializeComponent();
            DirectoryInfo di = new DirectoryInfo(@"D:\screenshots");
            var files = Directory.GetFiles(@"D:\screenshots", "*.png", SearchOption.TopDirectoryOnly);
            Array.Sort(files, delegate(string str1, string str2)
            {
                return File.GetCreationTime(str1).CompareTo(File.GetCreationTime(str2));
            });            

            foreach (string path in Directory.GetFiles(@"D:\screenshots", "*.png", SearchOption.TopDirectoryOnly))
            {
                var ct = File.GetCreationTime(path);
                images.Add(System.Drawing.Image.FromFile(path));
            }

            maxImages = images.Count - 1;
            SetImageSource();
            UpdateLabel();
        }
        
        private void SetImageSource()
        {
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
            images[currentImage].Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            image1.Source = bi;
        }
        
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage < maxImages)
                ++currentImage;
            SetImageSource();
            UpdateLabel();
        }
        
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (currentImage > 0)
                --currentImage;
            SetImageSource();
            UpdateLabel();
        }
        
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (maxImages > 10 && currentImage > 10)
                currentImage -= 10;
            else if (maxImages > 10 && currentImage < 10)
                currentImage = 0;
            else
                currentImage = 0;
            SetImageSource();
            UpdateLabel();
        }
        
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (maxImages > 10 && currentImage < (maxImages - 10))
                currentImage += 10;
            else if (maxImages > 10 && (currentImage > (maxImages - 10) && currentImage < maxImages))
                currentImage = maxImages;
            else
                currentImage = maxImages;
            SetImageSource();
            UpdateLabel();
        }
        
        private void UpdateLabel()
        {
            lblCount.Content = (currentImage + 1) + "/" + (maxImages + 1);
        }
    }
}
