using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;

namespace SerialTunningTool
{
    class Communication
    {
        private static SerialPort Com = null;
        private static byte[,] bytesBuffer = null;
        private Timer timer;
        private static int bufferCount = 0;
        public Communication(SerialPort com) {
            Com = com;
            bytesBuffer = new byte[128, 8];
            timer = new Timer();
            timer.Interval = 100;
            timer.Tick +=timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (bufferCount > 0)
                {
                    byte[] bytes = new byte[8];
                    for (int i = 0; i < 8; i++)
                    {
                        bytes[i] = bytesBuffer[bufferCount, i];
                    }
                    bufferCount--;
                    //Com.Write(bytes, 0, 8);
                }
            }
            catch { }
        }



        public static void SendCmd(byte cmd, float data)
        {
            try
            {
                byte[] bytes = new byte[4];
                int halfInt = MathTools.FloatToHalfInt(data);
	            bytes[0] = 0x24;
                bytes[1] = (byte)(cmd + 14);
                bytes[2] = (byte)(((halfInt & 0xff00) >> 8) + 1);
                bytes[3] = (byte)((halfInt & 0x00ff) + 1);

                Com.Write(bytes, 0, 4);
            }
            catch { }
        }
    }
}
