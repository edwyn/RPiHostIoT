﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.IO;
using System.ServiceModel;
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
                        await Dispatcher.RunAsync(
                              CoreDispatcherPriority.High,
                             () =>
                             {
                                 RetrieveImage();
                             });
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
                case "Unspecified":
                default:
                    {
                        // Do nothing 
                        break;
                    }
            }
        }

        //private void FlipLED()
        //{
        //    if (LEDStatus == 0)
        //    {
        //        LEDStatus = 1;
        //        if (pin != null)
        //        {
        //            // to turn on the LED, we need to push the pin 'low'
        //            pin.Write(GpioPinValue.Low);
        //        }
        //        LED.Fill = redBrush;
        //        StateText.Text = "On";
        //    }
        //    else
        //    {
        //        LEDStatus = 0;
        //        if (pin != null)
        //        {
        //            pin.Write(GpioPinValue.High);
        //        }
        //        LED.Fill = grayBrush;
        //        StateText.Text = "Off"; 
        //    }
        //}

        //private void TurnOffLED()
        //{
        //    if (LEDStatus == 1)
        //    {
        //        FlipLED();
        //    }
        //}
        //private void TurnOnLED()
        //{
        //    if (LEDStatus == 0)
        //    {
        //        FlipLED();
        //    }
        //}
        private void RetrieveImage()
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
            string filename = proxy.SendMessage(string.Format("{0}", Guid.NewGuid()));
            byte[] image = proxy.GetImageBytes(string.Format("{0}", Guid.NewGuid()));

            try
            {
                BitmapImage bitmap = new BitmapImage();
                MemoryStream mystream = new MemoryStream(image);
                var randomAccessStream = new MemoryRandomAccessStream(mystream);
                bitmap.SetSourceAsync(randomAccessStream);

                captureImage.Source = bitmap;
            }
            catch (Exception ex)
            {
                // status.Text = ex.Message;
                //Cleanup();
            }
        }

        private int LEDStatus = 0;
        private const int LED_PIN = 5;
        //private GpioPin pin;
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

    }
}
