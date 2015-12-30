using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FirstService
{
    class Program
    {
        static void Main(string[] args)
        {
            const string serviceURL = "net.tcp://192.168.0.109/MyFirstService";//"http://192.168.0.109:8732/Design_Time_Addresses/MyFirstService";

            var serviceHost = new ServiceHost(typeof(Service));
            

            serviceHost.Opening += ServiceHost_Opening;
            serviceHost.Opened += ServiceHost_Opened;
            serviceHost.Closing += ServiceHost_Closing;
            serviceHost.Closed += ServiceHost_Closed;

            var tcpBinding = new NetTcpBinding();
            tcpBinding.TransactionFlow = false;
            tcpBinding.Security.Transport.ProtectionLevel =
               System.Net.Security.ProtectionLevel.None;
            tcpBinding.Security.Transport.ClientCredentialType =
               TcpClientCredentialType.Windows;
            tcpBinding.Security.Mode = SecurityMode.None;
            
            /*http://www.codeproject.com/Articles/36973/Stream-Operation-in-WCF */

            tcpBinding.MaxReceivedMessageSize      = 3200000;
            tcpBinding.MaxBufferSize               = 3200000;
            tcpBinding.ReaderQuotas.MaxArrayLength = 3200000;
            tcpBinding.SendTimeout = TimeSpan.FromMinutes(2);
            
            
            

            serviceHost.AddServiceEndpoint(typeof(IService), tcpBinding, serviceURL);

            serviceHost.Open();

            Console.WriteLine("Host is open. Hit enter to close");

            Console.ReadLine();

            serviceHost.Close();
        }

        private static void ServiceHost_Closed(object sender, EventArgs e)
        {
            Console.WriteLine("Closed");
        }

        private static void ServiceHost_Closing(object sender, EventArgs e)
        {
            Console.WriteLine("Closing");
        }

        private static void ServiceHost_Opened(object sender, EventArgs e)
        {
            Console.WriteLine("Opened");
        }

        private static void ServiceHost_Opening(object sender, EventArgs e)
        {
            Console.WriteLine("Opening");
        }
    }
}
