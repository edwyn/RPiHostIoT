using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BlinkyWebService
{
   
    class BuildManager
    {
        private const int STATE_START = 0;
        private const int STATE_DO_NEXT_COMMAND = 1;
        private const int STATE_WAITING_FOR_DELAY = 2;
        private const int STATE_CANCELLED = 3;
        private const int STATE_IDLE = 4;
        private const int STATE_DONE = 5;
        private const int STATE_WAIT_DISPLAY = 6; // waiting for the display to finish

        public const int SLICE_NORMAL = 0;
        public const int SLICE_BLANK = -1;
        public const int SLICE_CALIBRATION = -2;
        public const int SLICE_SPECIAL = -3; // pulled from a plugin by name
        public const int SLICE_OUTLINE = -4; // this is a outline slice

        public bool Printing { get; set; }

        public SlicedFiles  slicefiles { get; set; }
        public GCodeFile gcode { get; set; }
        private int m_gcodeline = 0;
        private int m_curlayer = 0; // the current visible slice layer index #
        private bool m_running; // a var to control thread life
                                //int m_state = STATE_IDLE; // the state machine variable

        private MainPage m_main;

        private AxisControl m_zaxis;
        public int printing_state { get; set; }
        public bool PrintingProcess { get; set; }

        public void StartPrint(SlicedFiles sf, GCodeFile m_gcode, AxisControl zaxis, MainPage m_page)
        {

            if (Printing)
                return;

            slicefiles = sf;
            gcode = m_gcode;
            m_main = m_page;
            m_zaxis = zaxis;

            Printing = true;

            printing_state = STATE_START;
            PrintingProcess = true;

           // m_main.Printstarted(0);
            Process();

            //m_page.SetImage(0);

        }

        int GetTimerValue()
        {
            return Environment.TickCount;
        }

        private static int getvarfromline(String line)
        {
            try
            {
                int val = 0;
                line = line.Replace(';', ' '); // remove comments
                line = line.Replace(')', ' ');
                String[] lines = line.Split('>');
                if (lines[1].ToLower().Contains("blank"))
                {
                    val = -1; // blank screen
                }
                else if (lines[1].Contains("Special_"))
                {
                    val = -3; // special image
                }
                //else if (lines[1].Contains("outline"))
                //{ //;<slice> outline XXX
                //    // 
                //    val = SLICE_OUTLINE;
                //    //still need to pull the number
                //    String[] lns2 = lines[1].Trim().Split(' ');
                //    outlinelayer = int.Parse(lns2[1].Trim()); // second should be variable
                //}
                else
                {
                    String[] lns2 = lines[1].Trim().Split(' ');
                    val = int.Parse(lns2[0].Trim()); // first should be variable
                }

                return val;
            }
            catch (Exception ex)
            {
                //DebugLogger.Instance().LogError(line);
               // DebugLogger.Instance().LogError(ex);
                return 0;
            }
        }

        

        public BitmapImage MakeBlank(int xres, int yres)
        {
            BitmapImage m_blankimage = null; // a blank image to display
            SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            m_blankimage = new BitmapImage();//
          //  var i = BitmapImage.CreateOptionsProperty()
            //if (m_blankimage == null)  // blank image is null, create it
            //{
            //    // try to load it from the plug-in
            //    m_blankimage = UVDLPApp.Instance().GetPluginImage("Blank");
            //    //otherwise, create a new one
            //    if (m_blankimage == null)
            //    {
            //        m_blankimage = new Bitmap(xres, yres);
            //        using (Graphics gfx = Graphics.FromImage(m_blankimage))
            //        using (SolidBrush brush = new SolidBrush(Color.Black))
            //        {
            //            gfx.FillRectangle(brush, 0, 0, xres, yres);
            //        }
            //        m_blankimage.Tag = BuildManager.SLICE_BLANK;
            //    }
            //}
            return m_blankimage;
        }

        async public void Process()
        {
            int now = GetTimerValue();
            int nextlayertime = 0;

            while (PrintingProcess)
            {
                try
                {
                    switch (printing_state)
                    {
                        case BuildManager.STATE_START:
                            //start things off, reset some variables
                            printing_state = BuildManager.STATE_DO_NEXT_COMMAND;
                            m_gcodeline = 0;
                            break;
                        case BuildManager.STATE_DO_NEXT_COMMAND:
                            if (m_gcodeline >= gcode.Lines.Length)
                            {
                                //we're done..
                                printing_state = BuildManager.STATE_DONE;
                                continue;
                            }
                            string line = "";
                            // if the driver reports we're ready for the next command, or
                            // if we choose to ignore the driver ready status
                            // go through the gcode, line by line
                            line = gcode.Lines[m_gcodeline++];
                            line = line.Trim();

                            if (line.Length > 0) // if the line is not blank
                            {
                                // send  the line, whether or not it's a comment - this is for a reason....
                                // should check to see if the firmware is ready for another line

                                m_zaxis.SendCommandToDevice(line + "\r\n");
                                //Todo UVDLPApp.Instance().m_deviceinterface.SendCommandToDevice(line + "\r\n");

                                if (line.ToLower().Contains("<delay> ")) // get the delay
                                {
                                    int delaytime = getvarfromline(line);
                                   // m_main.PrintDelay(delaytime);
                                    nextlayertime = GetTimerValue() + delaytime;
                                    printing_state = STATE_WAITING_FOR_DELAY;
                                    continue;
                                }
                                else if (line.ToLower().Contains("<dispcmd>")) // display command
                                {
                                    //PerformDisplayCommand(line);
                                }
                                //else if (line.ToLower().Contains("<waitfordisplay>"))  // wait for display to be done
                                //{
                                //    m_state = BuildManager.STATE_WAIT_DISPLAY;
                                //}
                                //else if (line.ToLower().Contains("<auxcmd>")) //auxillary command to run a pre-defined sequence
                                //{
                                //    PerformAuxCommand(line);
                                //}
                                else if (line.ToLower().Contains("<slice> ")) //get the slice number
                                {
                                    int layer = getvarfromline(line);
                                    int curtype = BuildManager.SLICE_NORMAL; // assume it's a normal image to begin with
                                    if (layer == SLICE_BLANK)
                                    {
                                        m_main.SetImage(-1);
                                        curtype = BuildManager.SLICE_BLANK;
                                    }
                                    else
                                    {
                                        m_curlayer = layer;
                                        m_main.SetImage(m_curlayer);

                                        //communicate current layer to website
                                        m_main.SendSliceToHTTP(m_curlayer);
                                    }
                                }
                            }
                            break;
                        case BuildManager.STATE_WAITING_FOR_DELAY:
                            if (GetTimerValue() >= nextlayertime)
                            {
                                printing_state = BuildManager.STATE_DO_NEXT_COMMAND; // move onto next layer
                            }
                            else
                            {
                                //Thread.Sleep(1); //  sleep for 1 ms to eliminate unnecessary cpu usage.
                                Task.Delay(1).Wait(); //  sleep for 1 ms to eliminate unnecessary cpu usage.
                            }
                            break;
                        case BuildManager.STATE_DONE:
                            PrintingProcess = false;
                            m_running = false;
                            printing_state = BuildManager.STATE_IDLE;
                            break;
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

    }
}
