// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Devices.Gpio;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BlinkyWebService
{
    public sealed partial class MainPage : Page
    {
        AppServiceConnection appServiceConnection;

        public MainPage()
        {
            InitializeComponent();
            //InitGPIO();
            GpioStatus.Text = "GPIO pin not initialized - Disabled";
            InitAppSvc();
        }

        //private void InitGPIO()
        //{
        //    var gpio = GpioController.GetDefault();

        //    // Show an error if there is no GPIO controller
        //    if (gpio == null)
        //    {
        //        pin = null;
        //        GpioStatus.Text = "There is no GPIO controller on this device.";
        //        return;
        //    }

        //    pin = gpio.OpenPin(LED_PIN);
        //    pin.Write(GpioPinValue.High);
        //    pin.SetDriveMode(GpioPinDriveMode.Output);

        //    GpioStatus.Text = "GPIO pin initialized correctly.";
        //}

        private async void InitAppSvc()
        {
            // Initialize the AppServiceConnection
            appServiceConnection = new AppServiceConnection();
            //WebServer_hz258y3tkez3a
            //WebServer_1w720vyc4ccym
            //WebServer_1w720vyc4ccym
            //appServiceConnection.PackageFamilyName = "WebServer_hz258y3tkez3a";
            appServiceConnection.PackageFamilyName = "WebServer_1w720vyc4ccym";
            appServiceConnection.AppServiceName = "App2AppComService";

            m_sf = new SlicedFiles();
            m_gcode = new GCodeFile();

            // Send a initialize request 
            var res = await appServiceConnection.OpenAsync();
            if (res == AppServiceConnectionStatus.Success)
            {
                var message = new ValueSet();
                message.Add("Command", "Initialize");
                var response = await appServiceConnection.SendMessageAsync(message);
                if (response.Status != AppServiceResponseStatus.Success)
                {
                    throw new Exception("Failed to send message");
                }
                appServiceConnection.RequestReceived += OnMessageReceived;
            }
        }

        private async void OnMessageReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var message = args.Request.Message;
            string newState = message["State"] as string;
           // string startState = message["Start"] as string;
            switch (newState)
            {
                case "On":
                    {
                       // byte[][] images = null;
                        BitmapImage[] bitmaps = null;
                        await Dispatcher.RunAsync(
                              CoreDispatcherPriority.High,
                            () =>
                            {
                                try
                                {
                                    RetrieveData();

                                    
                                    BitmapImage bitmap;// =  BitmapImage();
                                    bitmaps = new BitmapImage[m_sf.NumSlices];
                                    m_sf.Images = new BitmapImage[m_sf.NumSlices];
                                    m_sf.Imgs = new List<BitmapImage>(m_sf.NumSlices);
                                    MemoryStream mystream;
                                    foreach (var slice in m_sf.Slices)
                                    {
                                        mystream = new MemoryStream(slice);
                                        var randomAccessStream = new MemoryRandomAccessStream(mystream);
                                        bitmap = new BitmapImage();
                                        bitmap.SetSourceAsync(randomAccessStream);
                                        m_sf.Imgs.Add(bitmap);
                                    }
                                    m_sf.Slices = null;
                                }
                                catch (Exception ex)
                                {
                                    // status.Text = ex.Message;
                                    //Cleanup();
                                }

                                GpioStatus.Text = "Print Ready";
                            });

                        //foreach (var item in m_sf.Imgs((value, i) => new { i, value }))
                        
                        //for (int j = 0; j < m_sf.NumSlices; j++)
                        //{
                        //    try
                        //    {
                        //        await Dispatcher.RunAsync(
                        //          CoreDispatcherPriority.High,
                        //         () =>
                        //         {
                        //             Task.Delay(2000).Wait();
                        //             captureImage.Source = m_sf.GetImageAt(j);//[j];
                        //             GpioStatus.Text = "Test Slice: " + string.Format("{0}", j);
                        //         });
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //    }
                        //}
                        break;
                    }
                case "Off":
                    {
                        //await Dispatcher.RunAsync(
                        //CoreDispatcherPriority.High,
                        //() =>
                        //{
                        //    TurnOffLED();
                        // });
                        break;
                    }
                case "Start":
                {
                        if(m_sf.Imgs != null)
                        { 
                            foreach (var image in m_sf.Imgs)
                            {
                                try
                                {
                                    await Dispatcher.RunAsync(
                                        CoreDispatcherPriority.High,
                                        () =>
                                        {
                                            Task.Delay(2000).Wait();
                                            captureImage.Source = image;
                                        //GpioStatus.Text = "Test Slice: " + string.Format("{0}", m_sf.Imgs.);
                                        });
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                    break;
                }
                case "Unspecified":
                default:
                    {
                        // Do nothing 
                        break;
                    }
            }
        }
        /// <summary>
        /// Retrieves data from Host(gcode, sliced images)
        /// </summary>
        private void RetrieveData()
        {
            //setup wcf service
            const string serviceURL = "net.tcp://192.168.0.109/MyFirstService";
            var tcpBinding = new NetTcpBinding();
            tcpBinding.Security.Transport.ClientCredentialType =
                TcpClientCredentialType.Windows;
            tcpBinding.Security.Mode = SecurityMode.None;
            tcpBinding.MaxReceivedMessageSize = 3200000;
            tcpBinding.MaxBufferSize = 3200000;
            tcpBinding.ReaderQuotas.MaxArrayLength = 3200000;
            tcpBinding.SendTimeout = TimeSpan.FromMinutes(2);


            var endpointAddress =
                new EndpointAddress(serviceURL);

            var channelFactory = new ChannelFactory<IService>(tcpBinding, endpointAddress);
            var proxy = channelFactory.CreateChannel();

            //step one get array of filenames
            //get byte arrays from files
            string[] filename = proxy.SendMessage(string.Format("{0}", Guid.NewGuid()));
            m_sf.Filenames = filename;

            //Receive Gcode file as string array, each item in the array representing a line
            string[] gcodelines = proxy.SendGcode(string.Format("{0}", Guid.NewGuid()));
            m_gcode.Lines = gcodelines;
            
            //Receive byte array per image and store 
            byte[][] images = new byte[filename.Length][];
            for (int i = 0; i < filename.Length; i++)
            {
                images[i] = proxy.GetImageBytes(string.Format("{0}", filename[i]));
            }
            m_sf.Slices = images;
        }

        private int LEDStatus = 0;
        private const int LED_PIN = 5;
        private SlicedFiles m_sf;
        private GCodeFile m_gcode = null; // a reference from the active gcode file
        //private GpioPin pin;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

    }
}
