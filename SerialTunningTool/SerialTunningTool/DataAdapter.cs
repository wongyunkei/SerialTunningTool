using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace SerialTunningTool
{
    public class DataAdapter
    {
        protected LinkedList<object> dataList;

        public DataAdapter(){
            dataList = new LinkedList<object>();
        }

        public void sendData(object dataArray)
        {
            lock (dataList)
            {
                dataList.AddLast(dataArray);
            }
        }

        public void sendData(object[] dataArray)
        {
            foreach (object t in dataArray)
            {
                lock (dataList)
                {
                    dataList.AddLast(t);
                }
            }
        }

        public object[] getData()
        {
            object [] dataArray;
            lock(dataList){
                dataArray = dataList.ToArray();
            }
            return dataArray;
        }

        public void clear()
        {
            lock (dataList)
            {
                dataList.Clear();
            }
        }
    }
}
