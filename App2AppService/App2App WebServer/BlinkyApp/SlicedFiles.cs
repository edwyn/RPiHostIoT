using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace BlinkyWebService
{
    public class SlicedFiles
    {
        private int m_numslices;
        private byte[][] m_slices;
        private BitmapImage[] m_bitmaps;
          
        private string[] m_filenames;

        public List<BitmapImage> Imgs { get; set; }

        public byte[][] Slices
        {
            get { return m_slices; }
            set { m_slices = value;
                  m_numslices = m_slices.Length;
            }
        }

        public BitmapImage[] Images { get; set; }
        //{
        //    get { return m_bitmaps; }
        //    set { m_bitmaps = value; }    
        //}

        public string[] Filenames
        {
            get { return m_filenames; }
            set { m_filenames = value; }
        }
        

        public int NumSlices
        {
            get { return m_numslices; }
            set { m_numslices = value; }
        }

        public BitmapImage GetImageAt(int currentlayer)
        {
            if (currentlayer < m_numslices)
            {
                return Images[currentlayer];// m_bitmaps.ElementAt(currentlayer);
            }
            else
            {
                return null;
            }
        }



        public byte[] GetSlicedImage(int currentlayer)
        {
            return m_slices.ElementAt(currentlayer);
        }



    }
}
