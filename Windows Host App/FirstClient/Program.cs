using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FirstClient
{
    class Program
    {
        static void Main(string[] args)
        {
            const string serviceURL = "http://192.168.0.109:8732/Design_Time_Addresses/MyFirstService";

            ChannelFactory<IMyFirstService> channelFactory =
                new ChannelFactory<IMyFirstService>(
                    new BasicHttpBinding(),
                    new EndpointAddress(serviceURL));

            IMyFirstService myFirstService = channelFactory.CreateChannel();

            Console.WriteLine("This console application is a client for the MyFirstService WCF service");
            Console.WriteLine(string.Format("It tries to use the service at address: {0}", serviceURL));
            Console.WriteLine();
            Console.Write("Write text to get number of words: ");

            string s = Console.ReadLine();
            int numberWords = myFirstService.GetNumberWords(s);

            Console.WriteLine(string.Format("Number of words: {0}", numberWords));
        }
    }
}
