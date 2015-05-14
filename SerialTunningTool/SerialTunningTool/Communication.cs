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

        unsafe static void floatToBytes(float f, byte[] bytes)
        {
            uint value = *((uint*)&f);
            bytes[3] = (byte)((value & 0xff000000) >> 24);
            bytes[2] = (byte)((value & 0x00ff0000) >> 16);
            bytes[1] = (byte)((value & 0x0000ff00) >> 8);
            bytes[0] = (byte)((value & 0x000000ff) >> 0);
        }

        public static void SendCmd(byte cmd, float data)
        {
            try
            {
                byte[] bytes = new byte[9];
                byte[] b = new byte[4];
                floatToBytes(data, b);
                bytes[0] = 0x24;
                bytes[1] = (byte)(cmd + 1);
                bytes[2] = (byte)(b[0] + 1);
                bytes[3] = (byte)(b[1] + 1);
                bytes[4] = (byte)(b[2] + 1);
                bytes[5] = (byte)(b[3] + 1);
                UInt16 checkSum = cmd;
                for (int i = 0; i < 4; i++)
                {
                    checkSum += b[i];
                }
                bytes[6] = (byte)(((checkSum & 0xff00) >> 8) + 1);
                bytes[7] = (byte)((checkSum & 0x00ff) + 1);
                bytes[8] = (byte)'\0';

                Com.Write(bytes, 0, 8);
                //for (int i = 0; i < 8; i++ )
                //{
                //    bytesBuffer[bufferCount, i] = bytes[i];
                //} 
                //bufferCount++;
            }
            catch { }
        }

        //public static void SendCmd(int cmd, int data)
        //{
        //    if (cmd >= 0 && cmd <= 31 && data <= 1023 && data >= -1024)
        //    {
        //        data += 1024;
        //        UInt16 raw_data = (UInt16)((cmd << 11) + data);
        //        byte[] b = { (byte)'$', (byte)((raw_data & 0xff00) >> 8), (byte)(raw_data & 0x00ff), (byte)'\n' };

        //        try
        //        {
        //            Com.Write(b, 0, 4);

        //        }
        //        catch { }
        //    }
        //}
    }
}
