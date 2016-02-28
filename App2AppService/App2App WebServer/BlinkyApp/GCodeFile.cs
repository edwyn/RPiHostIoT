using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlinkyWebService
{
    class GCodeFile
    {
        private string[] m_lines;

        public string[] Lines
        {
            get { return m_lines; }
            set { m_lines = value; }
        }
    }
}
