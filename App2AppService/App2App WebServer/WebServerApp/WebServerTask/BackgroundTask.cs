// Copyright (c) Microsoft. All rights reserved.


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.Foundation.Collections;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.AppService;
using Windows.System.Threading;
using Windows.Networking.Sockets;
using System.IO;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;

namespace WebServerTask
{
    public sealed class WebServerBGTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Associate a cancellation handler with the background task. 
            taskInstance.Canceled += OnCanceled;

            // Get the deferral object from the task instance
            serviceDeferral = taskInstance.GetDeferral();

            var appService = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            if (appService != null &&
                appService.Name == "App2AppComService")
            {
                appServiceConnection = appService.AppServiceConnection;
                appServiceConnection.RequestReceived += OnRequestReceived;
            }

        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var message = args.Request.Message;
            string command = message["Command"] as string;

            switch (command)
            {
                case "Initialize":
                    {
                        var messageDeferral = args.GetDeferral();
                        //Set a result to return to the caller
                        var returnMessage = new ValueSet();
                        server = new HttpServer(8000, appServiceConnection);
                        
                        IAsyncAction asyncAction = Windows.System.Threading.ThreadPool.RunAsync(
                            (workItem) =>
                            {
                                server.StartServer();
                            });
                        returnMessage.Add("Status", "Success");
                        var responseStatus = await args.Request.SendResponseAsync(returnMessage);
                        messageDeferral.Complete();
                        break;
                    }

                case "Quit":
                    {
                        //Service was asked to quit. Give us service deferral
                        //so platform can terminate the background task
                        serviceDeferral.Complete();
                        break;
                    }
                case "Slice":
                    {
                        var slice_number = message["SliceNumber"];
                        var total_slice_number = message["TotalSliceNumber"];
                        server.SliceName = (string)slice_number;
                        server.TotalSliceName = (string)total_slice_number;
                        break;
                    }
            }
        }
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //Clean up and get ready to exit
        }

        BackgroundTaskDeferral serviceDeferral;
        AppServiceConnection appServiceConnection;
        HttpServer server;
    }

    


    public sealed class HttpServer : IDisposable
    {
        // string offHtmlString = "<html><head><title>Blinky App</title></head><body><form action=\"blinky.html\" method=\"GET\"><input type=\"radio\" name=\"state\" value=\"on\" onclick=\"this.form.submit()\"> On<br><input type=\"radio\" name=\"state\" value=\"off\" checked onclick=\"this.form.submit()\"> Off<br><input type=\"radio\" name=\"state\" value=\"start\" onclick=\"this.form.submit()\"> Start</form></body></html>";
        // string onHtmlString = "<html><head><title>Blinky App</title></head><body><form action=\"blinky.html\" method=\"GET\"><input type=\"radio\" name=\"state\" value=\"on\" checked onclick=\"this.form.submit()\"> On<br><input type=\"radio\" name=\"state\" value=\"off\" onclick=\"this.form.submit()\"> Off<br><input type=\"radio\" name=\"state\" value=\"start\" onclick=\"this.form.submit()\"> Start</form></body></html>";
        // string startHtmlString = "<html><head><title>Blinky App</title></head><body><form action=\"blinky.html\" method=\"GET\"><input type=\"radio\" name=\"state\" value=\"on\"  onclick=\"this.form.submit()\"> On<br><input type=\"radio\" name=\"state\" value=\"off\" onclick=\"this.form.submit()\"> Off<br><input type=\"radio\" name=\"state\" value=\"start\" checked onclick=\"this.form.submit()\"> Start</form></body></html>";
        // private HttpParser m_this = new HttpParser();
        // private const string offHTMLString = m_this.getOffHTML();
        // string offHtmlString = HttpParser.HtmlFormatOff;
        // string onHtmlString = HttpParser.HtmlFormatOn;
        // string startHtmlString = HttpParser.HtmlFormatStart;
        string offHtmlString = "<html><head><title>Blinky App</title></head><body><form action=\"blinky.html\" method=\"GET\"><input type=\"radio\" name=\"state\" value=\"on\" onclick=\"this.form.submit()\"> On<br><input type=\"radio\" name=\"state\" value=\"off\" checked onclick=\"this.form.submit()\"> Off<br><input type=\"radio\" name=\"state\" value=\"start\" onclick=\"this.form.submit()\"> Start<br><input type=\"radio\" name=\"state\" value=\"test_on\" onclick=\"this.form.submit()\"> Test_On<br><input type=\"radio\" name=\"state\" value=\"test_off\" onclick=\"this.form.submit()\"> Test_Off</form><form action=\"\"><input type=\"button\" name=\"axis\" value=\"^ 10\" onclick=\"window.location.href=\'test.html?axis=up10.0\';\"/><br><br><input type=\"button\" value=\"^ 1.0\" onclick=\"window.location.href=\'test.html?axis=up01.0\';\"/><br><br><input type=\"button\" value=\"^ 0.1\" onclick=\"window.location.href=\'test.html?axis=up00.1\';\"/><br></form><form action=\"\"><input type=\"button\" name=\"axis\" value=\"v 0.1\" onclick=\"window.location.href=\'test.html?axis=down00.1\';\"/><br><br><input type=\"button\" value=\"v 1.0\" onclick=\"window.location.href=\'test.html?axis=down01.0\';\"/><br><br><input type=\"button\" value=\"v 10\" onclick=\"window.location.href=\'test.html?axis=down10.0\';\"/><br></form><iframe src='slice.html'></iframe></body></html>";
        string onHtmlString = "<html><head><title>Blinky App</title></head><body><form action=\"blinky.html\" method=\"GET\"><input type=\"radio\" name=\"state\" value=\"on\" checked onclick=\"this.form.submit()\"> On<br><input type=\"radio\" name=\"state\" value=\"off\" onclick=\"this.form.submit()\"> Off<br><input type=\"radio\" name=\"state\" value=\"start\" onclick=\"this.form.submit()\"> Start<br><input type=\"radio\" name=\"state\" value=\"test_on\" onclick=\"this.form.submit()\"> Test_On<br><input type=\"radio\" name=\"state\" value=\"test_off\" onclick=\"this.form.submit()\"> Test_Off</form><form action=\"\"><input type=\"button\" name=\"axis\" value=\"^ 10\" onclick=\"window.location.href=\'test.html?axis=up10.0\';\"/><br><br><input type=\"button\" value=\"^ 1.0\" onclick=\"window.location.href=\'test.html?axis=up01.0\';\"/><br><br><input type=\"button\" value=\"^ 0.1\" onclick=\"window.location.href=\'test.html?axis=up00.1\';\"/><br></form><form action=\"\"><input type=\"button\" name=\"axis\" value=\"v 0.1\" onclick=\"window.location.href=\'test.html?axis=down00.1\';\"/><br><br><input type=\"button\" value=\"v 1.0\" onclick=\"window.location.href=\'test.html?axis=down01.0\';\"/><br><br><input type=\"button\" value=\"v 10\" onclick=\"window.location.href=\'test.html?axis=down10.0\';\"/><br></form><iframe src='slice.html'></iframe></body></html>";
        string startHtmlString = "<html><head><title>Blinky App</title></head><body><form action=\"blinky.html\" method=\"GET\"><input type=\"radio\" name=\"state\" value=\"on\"  onclick=\"this.form.submit()\"> On<br><input type=\"radio\" name=\"state\" value=\"off\" onclick=\"this.form.submit()\"> Off<br><input type=\"radio\" name=\"state\" value=\"start\" checked onclick=\"this.form.submit()\"> Start<br><input type=\"radio\" name=\"state\" value=\"test_on\" onclick=\"this.form.submit()\"> Test_On<br><input type=\"radio\" name=\"state\" value=\"test_off\" onclick=\"this.form.submit()\"> Test_Off</form><form action=\"\"><input type=\"button\" name=\"axis\" value=\"^ 10\" onclick=\"window.location.href=\'test.html?axis=up10.0\';\"/><br><br><input type=\"button\" value=\"^ 1.0\" onclick=\"window.location.href=\'test.html?axis=up01.0\';\"/><br><br><input type=\"button\" value=\"^ 0.1\" onclick=\"window.location.href=\'test.html?axis=up00.1\';\"/><br></form><form action=\"\"><input type=\"button\" name=\"axis\" value=\"v 0.1\" onclick=\"window.location.href=\'test.html?axis=down00.1\';\"/><br><br><input type=\"button\" value=\"v 1.0\" onclick=\"window.location.href=\'test.html?axis=down01.0\';\"/><br><br><input type=\"button\" value=\"v 10\" onclick=\"window.location.href=\'test.html?axis=down10.0\';\"/><br></form><iframe src='slice.html'></iframe></body></html>";

        private const uint BufferSize = 8192;
        private int port = 8000;
        private readonly StreamSocketListener listener;
        private AppServiceConnection appServiceConnection;
        //public string m_slicenr;

        public String ConnectionType { get; set; }    

        private ObservableCollection<DeviceInformation> listOfDevices = new ObservableCollection<DeviceInformation>();

        private string slicename;
        public string SliceName
        {
            get
            {
                return this.slicename;
            }
            set
            {
                this.slicename = value;
            }
        }

        private string totalslicename;

        public string TotalSliceName
        {
            get
            {
                return this.totalslicename;
            }
            set
            {
                this.totalslicename = value;
            }
        }



        private async void ListAvailablePorts()
        {
            try
            {
                string aqs = SerialDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(aqs);
                
                offHtmlString = "<html>";
                for (int i = 0; i < dis.Count; i++)
                {
                    offHtmlString += dis[i].Name;
                    offHtmlString += "<br>";
                    //                  listOfDevices.Add(dis[i]);
                    //                dis
                }
                offHtmlString += "</html>";
                // var selectedPort = dis.First();
                /* serialPort = await SerialDevice.FromIdAsync(selectedPort.Id);
                

                 serialPort.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                 serialPort.BaudRate = 9600;
                 serialPort.Parity = SerialParity.None;
                 serialPort.StopBits = SerialStopBitCount.One;
                 serialPort.DataBits = 8;

                 infoBox.Text = "Serial port configured successfully!\n ----- Properties ----- \n";
                 infoBox.Text += "BaudRate: " + serialPort.BaudRate.ToString() + "\n";
                 infoBox.Text += "DataBits: " + serialPort.DataBits.ToString() + "\n";
                 infoBox.Text += "Handshake: " + serialPort.Handshake.ToString() + "\n";
                 infoBox.Text += "Parity: " + serialPort.Parity.ToString() + "\n";
                 infoBox.Text += "StopBits: " + serialPort.StopBits.ToString() + "\n";

                 data.Text = "configuring port";
                 */
            }

            catch (Exception ex)
            {
              //  infoBox.Text = "OOps, Something went wrong! \n" + ex.Message;
            }
        }

        public HttpServer(int serverPort, AppServiceConnection connection)
        {
            listener = new StreamSocketListener();
            port = serverPort; 
            appServiceConnection = connection;
            //m_slicenr = "-1";
            SliceName = "-1";
            TotalSliceName = "-1";
            listener.ConnectionReceived += (s, e) => ProcessRequestAsync(e.Socket);
        }

        public void StartServer()
        {
#pragma warning disable CS4014
            listener.BindServiceNameAsync(port.ToString());
#pragma warning restore CS4014
        }

        public void Dispose()
        {
            listener.Dispose();
        }

        private async void ProcessRequestAsync(StreamSocket socket)
        {
            // this works for text only
            StringBuilder request = new StringBuilder();
            using (IInputStream input = socket.InputStream)
            {
                byte[] data = new byte[BufferSize];
                IBuffer buffer = data.AsBuffer();
                uint dataRead = BufferSize;
                while (dataRead == BufferSize)
                {
                    await input.ReadAsync(buffer, BufferSize, InputStreamOptions.Partial);
                    request.Append(Encoding.UTF8.GetString(data, 0, data.Length));
                    dataRead = buffer.Length;
                }
            }

            using (IOutputStream output = socket.OutputStream)
            {
                string requestMethod = request.ToString().Split('\n')[0];
                string[] requestParts = requestMethod.Split(' ');

                if (requestParts[0] == "GET")
                    await WriteResponseAsync(requestParts[1], output);
                else
                    throw new InvalidDataException("HTTP method not supported: "
                                                   + requestParts[0]);
            }
        }

        private async Task WriteResponseAsync(string request, IOutputStream os)
        {
            // See if the request is for blinky.html, if yes get the new state
            string state = "Unspecified";
            string axisState = "Unspecified";
            string axisdn = "test.html?axis=down";
            char[] arr;
            string str = "";
            string axisValue = "";
            bool stateChanged = false;

            // ListAvailablePorts();

            string html = offHtmlString; // default off
            if (request.Contains("blinky.html?state=on"))
            {
                state = "On";
                stateChanged = true;
                html = onHtmlString;
            }
            else if (request.Contains("blinky.html?state=off"))
            {
                state = "Off";
                stateChanged = true;
                html = offHtmlString;
            }
            else if (request.Contains("blinky.html?state=start"))
            {
                state = "Start";
                stateChanged = true;
                html = startHtmlString;
            }
            else if (request.Contains("blinky.html?state=test_on"))
            {
                state = "Test_ON";
                stateChanged = true;
                html = offHtmlString;
            }
            else if (request.Contains("blinky.html?state=test_off"))
            {
                state = "Test_OFF";
                stateChanged = true;
                html = offHtmlString;
            }
            else if(request.Contains("test.html?axis="))
            {
                axisState = "On";
                stateChanged = true;
                
                string up = "test.html?axis=up";

                string sub = request.Substring(request.Length - 4, 4);
                if (request.Contains("down")==true)
                {

                    axisValue = "-";
                    //string str = request.Replace("test.html?axis=down", "");
                    axisValue += sub;
                } else
                {
                    axisValue = sub;
                }
            }
            else if(request.Contains("slice.html"))
            {
                //don't send any commands to backend just show current slice number
                html = "<html><head><meta http-equiv=\"refresh\" content=\"5\" /></head><body></body>Current Slice :" + SliceName +"/"+ TotalSliceName + "</html>";
                stateChanged = false;
            }
            

            if (stateChanged)
            {
                var updateMessage = new ValueSet();
                updateMessage.Add("State", state);
                updateMessage.Add("AxisState", axisState);
                updateMessage.Add("AxisValue", axisValue);
                //avar responseStatus = await appServiceConnection.SendMessageAsync(updateMessage);
                appServiceConnection.SendMessageAsync(updateMessage);
                
            }

            //string html = state == "On" ? onHtmlString : offHtmlString; 
            // Show the html 
            using (Stream resp = os.AsStreamForWrite())
                {
                    // Look in the Data subdirectory of the app package
                    byte[] bodyArray = Encoding.UTF8.GetBytes(html);
                    MemoryStream stream = new MemoryStream(bodyArray);
                    string header = String.Format("HTTP/1.1 200 OK\r\n" +
                                      "Content-Length: {0}\r\n" +
                                      "Connection: close\r\n\r\n",
                                      stream.Length);
                    byte[] headerArray = Encoding.UTF8.GetBytes(header);
                    await resp.WriteAsync(headerArray, 0, headerArray.Length);
                    await stream.CopyToAsync(resp);
                    await resp.FlushAsync();
                }
           
        }
     }
}
