using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace FirstService
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string[] SendMessage(string message);

        [OperationContract]
        byte[] GetImageBytes(string message);

        [OperationContract]
        string[] SendGcode(string message);
    }
}
