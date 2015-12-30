using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FirstService
{
    public class Service : IService
    {
        public string SendMessage(string message)
        {
            Console.WriteLine("Message is {0} received at {1}", message, DateTime.Now);
            return "tests0001.png";
        }

        public byte[] GetImageBytes(string message)
        {
            Console.WriteLine("GetImageBytes: Message is {0} received at {1}", message, DateTime.Now);
            string afilePath = @"C:\dev\git\wcf_tut\First\FirstService\images\tests0000.png";
            return System.IO.File.ReadAllBytes(afilePath);
        }

        public Stream GetFileStream(string message)
        {
            Console.WriteLine("GetFileStream: Message is {0} received at {1}", message, DateTime.Now);
            string afilePath = @"C:\dev\git\wcf_tut\First\FirstService\images\tests0000.png";
            FileStream stream = new FileStream(afilePath, FileMode.Open, FileAccess.Read);
            return stream;
        }

        public FileData GetFileData()
        {
           // string filePath = "C:\\dev\\git\\wcf_tut\\First\\FirstService\\images\\tests0000.png";
            string afilePath = @"C:\dev\git\wcf_tut\First\FirstService\images\tests0000.png";
            FileData streamData = new FileData();
            //byte[] image = System.IO.File.ReadAllBytes(afilePath);
            
            streamData.BufferData = System.IO.File.ReadAllBytes(afilePath);
            Console.WriteLine("Debug: File Size : " + streamData.BufferData.Length);
            streamData.FileName = "tests0001.png";
            streamData.FilePosition = 0;
            //streamData.BufferData = Convert.ToBase64String(System.IO.File.ReadAllBytes(filePath));
            return streamData;
        }

        
    }
}
