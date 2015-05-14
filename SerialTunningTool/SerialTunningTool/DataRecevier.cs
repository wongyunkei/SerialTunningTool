using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace SerialTunningTool
{
    class DataRecevier
    {
        private SerialPort Com = null;
        private DataAdapter[] Adapter = null;
        private String strBuffer = "";

        public DataRecevier(SerialPort com, DataAdapter [] adapter)
        {
            Com = com;
            Com.DataReceived += DataRecevier_DataReceived;
            Adapter = adapter;
        }

        unsafe float BytesTofloat(byte[] bytes)
        {
            uint value = ((uint)bytes[3] << 24) | ((uint)bytes[2] << 16) | ((uint)bytes[1] << 8) | ((uint)bytes[0] << 0);
            return *((float*)&value);
        }

        public void DataRecevier_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                strBuffer += Com.ReadExisting();
                if (strBuffer.Length >= 8)
                {
                    if (strBuffer.Contains("$"))
                    {
                        char[] ch = strBuffer.Substring(strBuffer.IndexOf('$') + 1).ToCharArray();
                        byte[] Bytes = new byte[7];
                        for (int i = 0; i < 7; i++)
                        {
                            Bytes[i] = (byte)(Convert.ToByte(ch[i]) - 1);
                        }
                        UInt16 checkSum = 0;
                        for (int j = 0; j < 5; j++)
                        {
                            checkSum += (UInt16)Bytes[j];
                        }
                        if (checkSum == (UInt16)((((UInt16)Bytes[5] << 8) | (UInt16)Bytes[6])))
                        {
                            int Cmd = Bytes[0];
                            byte[] b = { Bytes[1], Bytes[2], Bytes[3], Bytes[4] };
                            float Data = BytesTofloat(b);
                            Adapter[Cmd].sendData(Data);
                        }
                        strBuffer = "";
                    }
                }
            }
            catch { }


            //try
            //{

            //    String str = Com.ReadLine();
            //    if (!String.IsNullOrEmpty(str))
            //    {
            //        if (str.Contains('$'))
            //        {
            //            String[] s = str.Split(',');
            //            for (int i = 0; i < s.Length - 1; i++)
            //            {
            //                Adapter[i].sendData(Double.Parse(s[i + 1]));
            //            }
            //        }
            //        else
            //        {
            //            Adapter[Adapter.Length - 1].sendData(str);
            //        }
            //    }
            //}
            //catch { }
        }
    }

}
