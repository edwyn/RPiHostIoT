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
            //currently hardcoded VID_0403 PID_6001
            //usb = new UsbSerial("VID_2341", "PID_0010"); //Arduino available in Lantern
            usb = new UsbSerial("VID_0403", "PID_6001"); //Arduino available in LCD Printer
            usb.ConnectionEstablished += OnConnectionEstablished;
            //usb.begin(250000, SerialConfig.SERIAL_8N1);
            usb.begin(115200, SerialConfig.SERIAL_8N1);
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

            if(command.Contains("M114"))
            {
                bool isRead = true;
                ushort usbavt;
                byte t;
                List<byte> _ByteList = new List<byte>();

                ushort readBuf = usb.read();
                while (isRead)
                {
                    // usb.@lock();
                    usbavt = usb.available();
                    t = (byte)readBuf;
                    _ByteList.Add(t);
                    if (usbavt == 0)
                    {
                        //no more data to read
                        isRead = false;
                    }
                    readBuf = usb.read();
                }
            }

        }
    }
}
