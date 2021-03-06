﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace RaspberryPi
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        string SendMessage(string message);

        [OperationContract]
        Stream GetFileStream(string message);

        [OperationContract]
        FileData GetFileData();

        [OperationContract]
        byte[] GetImageBytes(string message);
    }
    [DataContract]
    public class FileData
    {
        [DataMember]
        public string FileName
        {
            get;
            set;
        }
        [DataMember]
        public byte[] BufferData
        {
            get;
            set;
        }
        [DataMember]
        public int FilePosition
        {
            get;
            set;
        }


    }


}
