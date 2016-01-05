using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace BlinkyWebService
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string[] SendMessage(string message);

        [OperationContract]
        byte[] GetImageBytes(string message);
    }
}
