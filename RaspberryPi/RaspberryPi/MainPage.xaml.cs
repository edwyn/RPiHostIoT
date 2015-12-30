using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using System.ServiceModel;
using Windows.Storage.Streams;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RaspberryPi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var uri = new Uri(this.BaseUri, "Assets/test0000.png");
            BitmapImage bmp = new BitmapImage();
            bmp.UriSource = uri;
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            PhotoCanvas.Width = bounds.Width;
            //PhotoCanvas.Height = bounds.Height;
            captureImage.Width = bounds.Width;
            //captureImage.Height = bounds.Height;
            captureImage.Source = bmp;

            
        }

        private void ClickMe_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void ClickMeSend_Click(object sender, RoutedEventArgs e)
        {
            //setup wcf service
            const string serviceURL = "net.tcp://192.168.0.109/MyFirstService";//"http://192.168.0.109:8732/Design_Time_Addresses/MyFirstService";
            var tcpBinding = new NetTcpBinding();
            tcpBinding.Security.Transport.ClientCredentialType =
              TcpClientCredentialType.Windows;
            tcpBinding.Security.Mode = SecurityMode.None;
            tcpBinding.MaxReceivedMessageSize      = 3200000;
            tcpBinding.MaxBufferSize               = 3200000;
            tcpBinding.ReaderQuotas.MaxArrayLength = 3200000;
            tcpBinding.SendTimeout = TimeSpan.FromMinutes(2);
            

            var endpointAddress =
                   new EndpointAddress(serviceURL);

            var channelFactory = new ChannelFactory<IService>(tcpBinding, endpointAddress);
            var proxy = channelFactory.CreateChannel();

            //step one get array of filenames
            //get byte arrays from files
            string filename = proxy.SendMessage(string.Format("{0}", Guid.NewGuid()));
            byte[] image = proxy.GetImageBytes(string.Format("{0}", Guid.NewGuid()));

            try
            {
              //  StorageFile photoFile;

              //  photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync(
               //         filename, CreationCollisionOption.GenerateUniqueName);
               // await FileIO.WriteBytesAsync(photoFile, image);
                //IRandomAccessStream photoStream = await photoFile.OpenReadAsync();
                BitmapImage bitmap = new BitmapImage();
                MemoryStream mystream = new MemoryStream(image);
                var randomAccessStream = new MemoryRandomAccessStream(mystream);
                bitmap.SetSourceAsync(randomAccessStream);
                //bitmap.SetSource(randomAccessStream);
                captureImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                // status.Text = ex.Message;
                //Cleanup();
            }


            HelloMessage.Text = HelloMessage.Text + " " + filename;
            using (proxy as IDisposable)
            {

            }
        }

        private static void ServiceHost_Closed(object sender, EventArgs e)
        {
            //Console.WriteLine("Closed");
        }

        private static void ServiceHost_Closing(object sender, EventArgs e)
        {
            //Console.WriteLine("Closing");
        }

        private static void ServiceHost_Opened(object sender, EventArgs e)
        {
            //Console.WriteLine("Opened");
        }

        private static void ServiceHost_Opening(object sender, EventArgs e)
        {
            //Console.WriteLine("Opening");
        }
    }
}
