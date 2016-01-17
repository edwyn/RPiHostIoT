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
        public string[] SendMessage(string message)
        {
            string[] filenames = new string[10];
            filenames[0] = "tests0000.png";
            filenames[1] = "tests0001.png";
            filenames[2] = "tests0002.png";
            filenames[3] = "tests0003.png";
            filenames[4] = "tests0004.png";
            filenames[5] = "tests0005.png";
            filenames[6] = "tests0006.png";
            filenames[7] = "tests0007.png";
            filenames[8] = "tests0008.png";
            filenames[9] = "tests0009.png";
            Console.WriteLine("Message is {0} received at {1}", message, DateTime.Now);
            return filenames;
        }

        public string[] SendGcode(string message)
        {
            string m_gcode = null;
            string[] gcodelines;
            string m_OutputFolder = @"C:\dev\git\wcf_tut\First\FirstService\images\";

            var files = Directory.EnumerateFiles(m_OutputFolder, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".gcode"));

            //Path to gcode file
            m_gcode = (string)files.First();

            //fill string arry with content gcode file
            gcodelines = File.ReadAllLines(m_gcode);

            return gcodelines;

        }

        public byte[] GetImageBytes(string message)
        {
            Console.WriteLine("GetImageBytes: Message is {0} received at {1}", message, DateTime.Now);
            string afilePath = @"C:\dev\git\wcf_tut\First\FirstService\images\" + message;//tests0000.png";
            return System.IO.File.ReadAllBytes(afilePath);
        }
    }
}
