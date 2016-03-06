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
            string m_OutputFolder = @"C:\dev\git\wcf_tut\First\FirstService\images2\";
            var files = Directory.EnumerateFiles(m_OutputFolder, "*.*", SearchOption.AllDirectories)
            .Where(s => s.EndsWith(".png"));
            string[] filenames = new string[files.Count()];
            int i = 0;
            foreach (var file in files)
            {
                filenames[i] = Path.GetFileName(file);
                i++;
            }

         /*   filenames[0] = "tests0000.png";
            filenames[1] = "tests0001.png";
            filenames[2] = "tests0002.png";
            filenames[3] = "tests0003.png";
            filenames[4] = "tests0004.png";
            filenames[5] = "tests0005.png";
            filenames[6] = "tests0006.png";
            filenames[7] = "tests0007.png";
            filenames[8] = "tests0008.png";
            filenames[9] = "tests0009.png";
            filenames[10] = "tests0010.png";
            filenames[11] = "tests0011.png";
            filenames[12] = "tests0012.png";
            filenames[13] = "tests0013.png";
            filenames[14] = "tests0014.png";
            filenames[15] = "tests0015.png";
            filenames[16] = "tests0016.png";
            filenames[17] = "tests0017.png";
            filenames[18] = "tests0018.png";
            filenames[19] = "tests0019.png";
            filenames[20] = "tests0020.png";
            filenames[21] = "tests0021.png";
            filenames[22] = "tests0022.png";
            filenames[23] = "tests0023.png";
            filenames[24] = "tests0024.png";
            filenames[25] = "tests0025.png";
            filenames[26] = "tests0026.png";
            filenames[27] = "tests0027.png";
            filenames[28] = "tests0028.png";
            filenames[29] = "tests0029.png";
            filenames[30] = "tests0030.png";
            filenames[31] = "tests0031.png";
            filenames[32] = "tests0032.png";
            filenames[33] = "tests0033.png";
            filenames[34] = "tests0034.png";
            filenames[35] = "tests0035.png";
            filenames[36] = "tests0036.png";
            filenames[37] = "tests0037.png";
            filenames[38] = "tests0038.png";
            filenames[39] = "tests0039.png";
            filenames[40] = "tests0040.png";
            filenames[41] = "tests0041.png";
            filenames[42] = "tests0042.png";
            filenames[43] = "tests0043.png";
            filenames[44] = "tests0044.png";
           */



            Console.WriteLine("Message is {0}, number of files {1} received at {2}", message, files.Count().ToString(), DateTime.Now);
            return filenames;
        }

        public string[] SendGcode(string message)
        {
            string m_gcode = null;
            string[] gcodelines;
            string m_OutputFolder = @"C:\dev\git\wcf_tut\First\FirstService\images2\";

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
            string afilePath = @"C:\dev\git\wcf_tut\First\FirstService\images2\" + message;//tests0000.png";
            return System.IO.File.ReadAllBytes(afilePath);
        }
    }
}
