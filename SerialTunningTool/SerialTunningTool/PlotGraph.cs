using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialTunningTool
{
    class PlotGraph
    {
        private Mitov.PlotLab.Scope _scope = null;
        private DataAdapter[] Adapter = null;
        private int ChannelsNum = 0;
        private TextBox _textBox = null;
        private int xMax = 200;
        private double[] data = null;

        public PlotGraph(Mitov.PlotLab.Scope scope, DataAdapter[] adapter, int channelsNum, TextBox textBox)
        {
            _scope = scope;  
            Adapter = adapter;
            _textBox = textBox;
            ChannelsNum = channelsNum;
            data = new double[ChannelsNum];
            for (int i = 0; i < ChannelsNum; i++)
            {
                _scope.Channels.Add();
                _scope.Channels[i].ChannelMode = Mitov.PlotLab.ScopeChannelMode.Line;
                _scope.Channels[i].Data.SetYData(new double[xMax]);
            }
            Timer timer = new Timer();
            timer.Tick += timer_Tick;
            timer.Interval = 10;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            plot();
        }

        public void clear() {
            for (int i = 0; i < ChannelsNum; i++)
            {
                _scope.Channels[i].Data.Clear();
                _scope.Channels[i].Data.SetYData(new double[xMax]);
            }
        }

        public void plot(){
            for (int i = 0; i < ChannelsNum; i++)
            {
                object[] b = Adapter[i].getData();
                if (b.Length > 0)
                {
                    try
                    {
                        double[] d = _scope.Channels[i].Data.GetYData();
                        for (int j = 0; j < xMax - b.Length; j++)
                        {
                            d[j] = d[j + b.Length];
                        }
                        for (int j = 0; j < b.Length; j++)
                        {
                            d[xMax - b.Length] = Convert.ToDouble((float)b[j]);
                        }
                        _scope.Channels[i].Data.SetYData(d);
                        data[i] = d[d.Length - 1];                        
                    }
                    catch { }
                }
                Adapter[i].clear();
            }
            _textBox.Clear();
            for (int i = 0; i < ChannelsNum; i++)
            {
                _textBox.Text += i + ": " + ((float)(int)(data[i] * 1000)) / 1000.0 + "\r\n";
            }
            _textBox.SelectionStart = _textBox.TextLength;
            _textBox.ScrollToCaret();
        }
    }
}
