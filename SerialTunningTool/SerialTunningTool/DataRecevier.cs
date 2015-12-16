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



        public void DataRecevier_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                strBuffer += Com.ReadExisting();
                if (strBuffer.Length >= 4)
                {
                    if (strBuffer.Contains("$"))
                    {
                        char[] ch = strBuffer.Substring(strBuffer.IndexOf('$') + 1).ToCharArray();
                        byte[] Bytes = new byte[3];
                        for (int i = 0; i < 3; i++)  
                        {
                            Bytes[i] = (byte)(Convert.ToByte(ch[i]) - 1);
                        }

                        int halfInt = (int)(Bytes[1] << 8) | (int)Bytes[2];
                       
                        int Cmd = Bytes[0] - 13;
                        float Data = MathTools.HalfIntToFloat(halfInt);
                        Adapter[Cmd].sendData(Data);
                        strBuffer = "";
                        if(Cmd < 3){
                            Adapter[Cmd+6].sendData(Data);
                        }
                    }
                }
            }
            catch { }
        }
    }

}
