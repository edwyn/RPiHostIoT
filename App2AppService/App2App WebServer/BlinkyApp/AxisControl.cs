using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Microsoft.Maker.Serial;

namespace BlinkyWebService
{
    class AxisControl
    {
        // private SerialDevice serialPort = null;
        private UsbSerial usb = null;
        public bool UsbConnectionType { get; set; }



        public void InitializeConnection(string vid, string pid)
        {
            //currently hardcoded
            usb = new UsbSerial("VID_2341", "PID_0010"); //Arduino available in Lantern
            usb.ConnectionEstablished += OnConnectionEstablished;
            usb.begin(250000, SerialConfig.SERIAL_8N1);
        }

        private void OnConnectionEstablished()
        {
            //update website connection made
            UsbConnectionType = true;

        }

        public void SendCommandToDevice(String command)
        {
            ushort usbcheck;
            if (usb.connectionReady())
            {
                var buffer = Encoding.ASCII.GetBytes(command).ToArray();
                usbcheck = usb.write(buffer);
                usb.flush();
            }

        }
    }
}
