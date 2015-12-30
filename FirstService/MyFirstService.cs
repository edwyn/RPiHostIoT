using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstService
{
    public class MyFirstService : IMyFirstService
    {
        public int GetNumberWords(string s)
        {
            return s.Split(
                new char[] { ' ', ';', ',', '.', '?' },
                StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }
}
