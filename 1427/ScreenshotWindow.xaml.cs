using SDL2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;



/*  TODO:
 *  Uses a massive amount of memory. Perhaps this should load an image on the fly
 *  when a button is pressed rather than loading them all into memory. 
 */

namespace PDTUtils
{
    /// <summary>
    /// Interaction logic for ScreenshotWindow.xaml
    /// </summary>
    public partial class ScreenshotWindow : Window
    {
        //List<System.Drawing.Image> images = new List<System.Drawing.Image>();
        List<IntPtr> images = new List<IntPtr>();
        int currentImage = 0;
        int maxImages = 0;

        public ScreenshotWindow()
        {
            InitializeComponent();
            try
            {
                SDL2.SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
                SDL2.SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_JPG | SDL_image.IMG_InitFlags.IMG_INIT_PNG);
                var files = Directory.GetFiles(@"D:\screenshots", "*.png", SearchOption.TopDirectoryOnly);
                Array.Sort(files, delegate(string str1, string str2)
                {
                    return File.GetCreationTime(str1).CompareTo(File.GetCreationTime(str2));
                });

                foreach (string path in Directory.GetFiles(@"D:\screenshots", "*.png", SearchOption.TopDirectoryOnly))
                {
                    var ct = File.GetCreationTime(path);
                    images.Add(SDL2.SDL_image.IMG_Load(path));
                }

                maxImages = images.Count - 1;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
          //  SDL2.SDL_image.IMG_Init(SDL_image.IMG_InitFlags.IMG_INIT_JPG | SDL_image.IMG_InitFlags.IMG_INIT_PNG);
            /*DirectoryInfo di = new DirectoryInfo(@"D:\screenshots");
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

            maxImages = images.Count - 1;*/
            SetImageSource();
            UpdateLabel();
        }
        
        private void SetImageSource()
        {
            /*BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            MemoryStream ms = new MemoryStream();
            images[currentImage].Save(ms, ImageFormat.Png);
            ms.Seek(0, SeekOrigin.Begin);
            bi.StreamSource = ms;
            bi.EndInit();
            image1.Source = bi;*/
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
